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
        for (var x = 0; x < Width; x++)
        {
            for (var y = 0; y < Height; y++)
            {
                var current = Instantiate(SingleHexagonIceFloe, CalcPos(x, y), Quaternion.identity, parent);
                current.name = $"[{x}:{y}]IceFloe";
                if (x == 0 || x == Width - 1 || y == 0 || y == Height - 1)
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

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
