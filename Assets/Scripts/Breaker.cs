using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Breaker : MonoBehaviour
{


    public GameObject[] ObjsToDestroy;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.D))
        {
            if (ObjsToDestroy != null && ObjsToDestroy.Length > 0)
            {
                var obj = ObjsToDestroy[ObjsToDestroy.Length - 1].GetComponents<FixedJoint>();
                foreach (var item in obj)
                {
                    Destroy(item);
                }
                Array.Resize(ref ObjsToDestroy, ObjsToDestroy.Length - 1);
            }
        }
    }
}
