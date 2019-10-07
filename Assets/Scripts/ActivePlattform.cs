using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivePlattform : MonoBehaviour
{

    public PlatformManager ActiveFloe;

    // Update is called once per frame
    void Update()
    {
        if (ActiveFloe) ActiveFloe.Falling -= ActiveFloe_Falling;
        if (Physics.Raycast(new Ray(transform.position, Vector3.down), out var hit, 1F))
        {
            ActiveFloe = hit.collider.GetComponentInParent<PlatformManager>();
            if (ActiveFloe)
                ActiveFloe.Falling += ActiveFloe_Falling;
        }
    }

    private void ActiveFloe_Falling(object sender, System.EventArgs e)
    {
        GetComponent<Rigidbody>().useGravity = true;
    }
}
