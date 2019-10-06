using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JointChecker : MonoBehaviour
{

    private FixedJoint[] _joints;

    private void OnJointBreak(float breakForce)
    {
        Debug.LogWarning($"BreakForce: {breakForce}");
    }

    // Start is called before the first frame update
    void Start()
    {
        _joints = GetComponents<FixedJoint>();
    }

    // Update is called once per frame
    void Update()
    {
        if(_joints == null)
        {
            _joints = GetComponents<FixedJoint>();
        }
        foreach (var item in _joints)
        {
            if (item)
                Debug.Log($"Obj:{item.connectedBody.name} : force:{item.currentForce} : torque:{item.currentTorque}");
        }
    }
}
