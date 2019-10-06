using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;

public class PlatformManager : MonoBehaviour
{
    private const float MIN_DROP_DISTANCE = 0.1F;
    private Dictionary<Transform, Vector3> _adjacents = new Dictionary<Transform, Vector3>();
    private System.Random _myRand = new System.Random();
    private Transform[] _orderedAdjacents;

    public int AdjacentsCount = 6;
    [Range(0f, 1f)]
    public float FallProbability = 0.9F;

    private void OnEnable()
    {
        var myCollider = GetComponentInChildren<Collider>();
        var hits = Physics.SphereCastAll(new Ray(myCollider.bounds.center + (Vector3.down * myCollider.bounds.extents.y * 2F), Vector3.up), myCollider.bounds.extents.magnitude * 1.1F, 2F);
        foreach (var item in hits)
        {
            if (item.collider == myCollider) continue;
            if ((item.collider.gameObject.layer & 8) == 8) // Pinguin
            {
                continue;
            }
            _adjacents[item.transform] = item.transform.position;
        }
        var myPos = transform.position;
        _orderedAdjacents = _adjacents.OrderBy(entry => Vector3.SignedAngle(entry.Value - myPos, Vector3.forward, Vector3.up)).Select(entry => entry.Key).ToArray();
        AdjacentsCount = _adjacents.Count;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        var dropped = (from adj in _adjacents
                       where Vector3.Distance(adj.Key.position, adj.Value) > MIN_DROP_DISTANCE
                       select adj.Key).ToList();
        var newDropped = false;
        foreach (var item in dropped)
        {
            newDropped = true;
            _adjacents.Remove(item);
        }
        if (newDropped)
        {
            AdjacentsCount = _adjacents.Count;
            ShouldFall();
        }
    }

    private void ShouldFall()
    {
        var tries = 5;
        for (int i = 0; i < 3; i++)
        {
            //two on opposite 
            if (_orderedAdjacents.Length > i + 3 && _adjacents.ContainsKey(_orderedAdjacents[i]) && _adjacents.ContainsKey(_orderedAdjacents[i + 3]))
            {
                tries--;
            }
        }
        for (int i = 0; i < 2; i++)
        {
            //three in triangle 
            if (_orderedAdjacents.Length > i + 4 && _adjacents.ContainsKey(_orderedAdjacents[i]) && _adjacents.ContainsKey(_orderedAdjacents[i + 2]) && _adjacents.ContainsKey(_orderedAdjacents[i + 4]))
            {
                tries--;
            }
        }

        for (int i = 0; i < tries; i++)
        {
            float v = UnityEngine.Random.Range(0F, 1F);
            if (v > FallProbability)
            {

                Debug.Log($"{name}: falling (Tries:{tries} Prob:{v})");
                GetComponent<Rigidbody>().useGravity = true;
                return;
            }
        }
        Debug.Log($"{name}: not falling");
    }
}
