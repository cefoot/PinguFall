using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;

public class PlatformManager : MonoBehaviour
{

    private const float MIN_DROP_DISTANCE = 0.1F;
    private readonly Dictionary<Transform, Vector3> _adjacents = new Dictionary<Transform, Vector3>();
    private Transform[] _orderedAdjacents;
    private Rigidbody _myRigid;

    public float MinProbabilityRange = 0.55F;
    public float MaxProbabilityRange = 1F;
    public int AdjacentsCount = 6;
    [Range(0f, 1f)]
    public float FallProbability = 0.08F;
    public float LastProbability = 1F;

    private void OnEnable()
    {
        InitAdjacents();
    }

    public void InitAdjacents()
    {
        _adjacents.Clear();
        _myRigid = GetComponent<Rigidbody>();
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

    // Update is called once per frame
    void Update()
    {
        if (_myRigid.useGravity) Drop();
        var dropped = (from adj in _adjacents
                       where adj.Key && Vector3.Distance(adj.Key.position, adj.Value) > MIN_DROP_DISTANCE
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

    public void Drop()
    {
        _myRigid.useGravity = true;
        enabled = false;
        _myRigid.isKinematic = false;
    }


    private void ShouldFall()
    {
        var tries = 10;
        for (var i = 0; i < 3; i++)
        {
            //two on opposite 
            if (_orderedAdjacents.Length > i + 3 && _adjacents.ContainsKey(_orderedAdjacents[i]) && _adjacents.ContainsKey(_orderedAdjacents[i + 3]))
            {
                tries -= 2;
            }
        }
        for (var i = 0; i < 2; i++)
        {
            //three in triangle 
            if (_orderedAdjacents.Length > i + 4 && _adjacents.ContainsKey(_orderedAdjacents[i]) && _adjacents.ContainsKey(_orderedAdjacents[i + 2]) && _adjacents.ContainsKey(_orderedAdjacents[i + 4]))
            {
                tries -= 2;
            }
        }
        var probs = new List<float>();
        LastProbability = 1F;
        for (var i = 0; i < tries; i++)
        {
            var v = UnityEngine.Random.Range(MinProbabilityRange, MaxProbabilityRange);
            probs.Add(v);
            LastProbability *= v;
        }
        if (LastProbability < FallProbability)
        {
            Debug.Log($"{name}: falling ([{probs.Count}]{String.Concat(probs.ToArray())}:\r\n{LastProbability})", this);
            Drop();
        }
        //else
        //{
        //    Debug.Log($"{name}: NOT falling ([{probs.Count}]{String.Concat(probs.ToArray())}:\r\n{prob})");
        //}
    }
}
