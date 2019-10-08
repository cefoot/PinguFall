using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceFloe : MonoBehaviour
{

    public int Height = 10;
    public int Width = 10;
    public Transform SingleHexagonIceFloe;
    private const float ADDITIONAL_OFFSET_Y = 0.25F;
    private const float OFFSET_Y = 0.5F;
    private const float OFFSET_X = 0.4330127F;

    private void OnEnable()
    {
        var parent = transform;
        var minX = Width / -2;
        var maxX = Width / 2;
        var minY = Height / -2;
        var maxY = Height / 2;
        for (var x = minX; x < maxX; x++)
        {
            for (var y = minY; y < maxY; y++)
            {
                var current = Instantiate(SingleHexagonIceFloe, parent.position + CalcPos(x, y), Quaternion.identity, parent);
                current.name = $"[{x}:{y}]IceFloe";
                if (x == minX || x == maxX - 1 || y == minY || y == maxY - 1)
                {
                    Destroy(current.GetComponent<PlatformManager>());
                    var bdy = current.GetComponent<Rigidbody>();
                    bdy.isKinematic = true;
                }
            }
        }
        BroadcastMessage("InitAdjacents", SendMessageOptions.DontRequireReceiver);
    }

    private Vector3 CalcPos(int x, int y)
    {
        return new Vector3(x * OFFSET_X, 0F, y * OFFSET_Y + (x % 2) * ADDITIONAL_OFFSET_Y);
    }

    public void Reset()
    {
        StartCoroutine(ResetProcess());
    }

    private IEnumerator ResetProcess()
    {
        foreach (Transform trans in transform)
        {
            Destroy(trans.gameObject);
        }
        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();
        OnEnable();
    }

}
