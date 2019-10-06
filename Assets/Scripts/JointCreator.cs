using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using System.Collections.Concurrent;

public class JointCreator : MonoBehaviour
{
    [Serializable]
    public class Mem
    {
        public string From;
        public string To;
        public float MaxForce;
        public string ToString()
        {
            return $"[{From}->{To}]:{MaxForce}";
        }
    }

    public float TorqueMultiplier = 2F;
    public float ForceMultiplier = 2F;
    public float BreakTorque = 2F;
    public float BreakForce = 2F;
    public float LiniearSpring = 10F;
    public static ConcurrentDictionary<Rigidbody, List<Joint>> _bdyToJnt = new ConcurrentDictionary<Rigidbody, List<Joint>>();
    private List<ConfigurableJoint> _myJoints = new List<ConfigurableJoint>();
    private Rigidbody _myBdy;
    public int CreatedJoints = 0;
    private readonly Vector3 HALF_HEIGHT = new Vector3(0f, -0.07744f, 0f);
    public Mem[] MaxForces;

    private void OnJointBreak(float breakForce)
    {
        var jnts = GetComponents<Joint>();
        foreach (var item in jnts)
        {
            if (item.currentForce.magnitude == breakForce)
            {
                Debug.DrawLine(item.connectedBody.transform.position, item.transform.position, Color.cyan, 10F);
                Debug.LogWarning($"[{item.name}->{item.connectedBody.name}]BreakForce: {breakForce}:{item.breakForce}");
                Destroy(item);
                break;
            }
            if (item.currentTorque.magnitude == breakForce)
            {
                Debug.DrawLine(item.connectedBody.transform.position, item.transform.position, Color.red, 10F);
                Debug.LogWarning($"[{item.name}->{item.connectedBody.name}]BreakTorque: {breakForce}:{item.breakTorque}");
                Destroy(item);
                break;
            }
        }
    }

    private void OnEnable()
    {
        var addHits = new List<Rigidbody>();
        _myBdy = GetComponent<Rigidbody>();
        var hits = Physics.SphereCastAll(new Ray(transform.position + Vector3.down, Vector3.up), 0.5F, 2F);
        Collider myCollider = GetComponentInChildren<Collider>();
        foreach (var item in hits)
        {
            if (item.collider == myCollider) continue;
            if ((item.collider.gameObject.layer & 8) == 8)
            {
                continue;
            }
            var rigid = item.collider.GetComponentInParent<Rigidbody>();
            if (!_bdyToJnt.ContainsKey(rigid))
            {
                _bdyToJnt[rigid] = new List<Joint>();
            }
            if (addHits.Contains(rigid)) continue;
            addHits.Add(rigid);

            var jnt = gameObject.AddComponent<ConfigurableJoint>();
            _myJoints.Add(jnt);
            CreatedJoints++;
            try
            {
                if (jnt.gameObject == rigid.gameObject)
                {
                    Destroy(jnt);
                    continue;
                }
                jnt.connectedBody = rigid;
                _bdyToJnt[rigid].Add(jnt);
            }
            catch (Exception)
            {
                Debug.LogError($"Error:{rigid.gameObject.name}+{gameObject.name}");
                throw;
            }
            jnt.autoConfigureConnectedAnchor = false;
            var dir = jnt.transform.position - jnt.connectedBody.transform.position;
            jnt.connectedAnchor = jnt.connectedBody.transform.InverseTransformPoint(jnt.connectedBody.transform.position + (dir / 2F) + HALF_HEIGHT);
            jnt.anchor = jnt.transform.InverseTransformPoint(jnt.transform.position + (dir / -2F) + HALF_HEIGHT);
            jnt.xMotion = ConfigurableJointMotion.Limited;
            jnt.yMotion = ConfigurableJointMotion.Limited;
            jnt.zMotion = ConfigurableJointMotion.Limited;
            jnt.angularXMotion = ConfigurableJointMotion.Locked;
            jnt.angularYMotion = ConfigurableJointMotion.Locked;
            jnt.angularZMotion = ConfigurableJointMotion.Locked;
            jnt.linearLimit = new SoftJointLimit
            {
                limit = 0.00001F,
            };
            jnt.linearLimitSpring = new SoftJointLimitSpring
            {
                spring = LiniearSpring,
            };
            //jnt.spring = 100F;
            //jnt.damper = 0F;
            //jnt.tolerance = 0F;
            //jnt.enableCollision = true;
            jnt.breakForce = BreakForce;
        }
        //Invoke("ForceResetBreakForces", 2F);
        Invoke("SetMaxForces", 1F);
    }

    public void SetMaxForces()
    {
        MaxForces = (from jnt in GetComponents<Joint>()
                     orderby jnt.connectedBody.name
                     select new Mem
                     {
                         From = jnt.name,
                         To = jnt.connectedBody.name,
                         MaxForce = jnt.currentForce.magnitude,
                     }).ToArray();
        //InvokeRepeating("UpdateMaxForces", 1F, 1F);
    }

    public void UpdateMaxForces()
    {
        foreach (var item in (from jnt in GetComponents<Joint>()
                              orderby jnt.connectedBody.name
                              join p in MaxForces on jnt.connectedBody.name equals p.To
                              where p.MaxForce < jnt.currentForce.magnitude
                              select new { Joint = jnt, Data = p }))
        {
            item.Data.MaxForce = item.Joint.currentForce.magnitude;
        }
    }

    private void ResetBreakForces()
    {
        ResetBreakForces(false);
    }

    private void ForceResetBreakForces()
    {
        ResetBreakForces(true);
    }

    private void ResetBreakForces(bool forceUpdate)
    {
        if (_bdyToJnt.ContainsKey(_myBdy))
            _bdyToJnt[_myBdy].ForEach(jnt =>
            {
                if (forceUpdate)
                {
                    jnt.breakForce = (jnt.currentForce.magnitude > 0 ? jnt.currentForce.magnitude : 1) * ForceMultiplier;
                    jnt.breakTorque = (jnt.currentTorque.magnitude > 0 ? jnt.currentTorque.magnitude : 1) * TorqueMultiplier;
                }
            });
    }

    private void OnDestroy()
    {
        _bdyToJnt[_myBdy].ForEach(jnt => Destroy(jnt));
        List<Joint> lst;
        _bdyToJnt.TryRemove(_myBdy, out lst);
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(DrawJoints());
    }

    private IEnumerator DrawJoints()
    {
        while (this)
        {
            if (_bdyToJnt.ContainsKey(_myBdy))
                foreach (var item in _bdyToJnt[_myBdy])
                {
                    if (!item) continue;
                    Debug.DrawLine(item.connectedBody.transform.position, transform.position, Color.red, 1F);
                }
            yield return new WaitForSeconds(1f);
        }
    }

    float maxFrc = 0F;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.M))
        {
            var rend = GetComponentInChildren<Renderer>();
            Debug.Log($"{name}:{rend.bounds.extents.ToString("G4")}");
        }
        //var jnt = GetComponent<Joint>();
        //if (jnt)
        //{
        //    maxFrc = Mathf.Max(maxFrc, jnt.currentForce.magnitude);
        //    Debug.DrawLine(jnt.connectedBody.transform.position, jnt.transform.position, new Color(jnt.currentForce.magnitude / 10F, 0F, 0F));
        //    Debug.LogWarning($"[{jnt.name}->{jnt.connectedBody.name}]CurrentForce: {jnt.currentForce.magnitude}:{jnt.breakForce}|{maxFrc}");
        //}
        UpdateMaxForces();
        foreach (var jnts in _myJoints)
        {
            if (jnts)
                jnts.linearLimitSpring = new SoftJointLimitSpring { spring = LiniearSpring, };
        }
    }
}
