using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PenguinWalk : MonoBehaviour
{
    private ActivePlattform _platform;
    private Vector3? _target = null;

    // Start is called before the first frame update
    void Start()
    {
        _platform = GetComponent<ActivePlattform>();
        InvokeRepeating("Walk", 3f, 3f);
    }

    public void Walk()
    {
        var neighbours = _platform.ActiveFloe.OrderedAdjacents;
        var target = Random.Range(0, neighbours.Length + 5);
        if (target >= neighbours.Length) return;//stay here
        //play animation
    }

    private void Update()
    {
        if(_target != null)
        {
            //walk
        }
    }
}
