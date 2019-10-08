using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PenguController : MonoBehaviour
{

    public PlatformManager ActiveFloe;
    private Vector3 _startPos;
    public UnityEvent PenguFalling;

    private void Awake()
    {
        _startPos = transform.localPosition;
    }

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

    public void Reset()
    {
        StartCoroutine(ResetProcess());
    }

    private IEnumerator ResetProcess()
    {
        Rigidbody rigidbody = GetComponent<Rigidbody>();
        rigidbody.useGravity = false;
        rigidbody.isKinematic = true;
        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();
        rigidbody.velocity = Vector3.zero;
        rigidbody.angularVelocity = Vector3.zero;
        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();
        transform.localPosition = _startPos;
        transform.localRotation = Quaternion.identity;
        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();
        rigidbody.isKinematic = false;
        GetComponent<PenguinWalk>().enabled = true;
    }

    private void ActiveFloe_Falling(object sender, System.EventArgs e)
    {
        GetComponent<Rigidbody>().useGravity = true;
        GetComponent<PenguinWalk>().enabled = false;
        PenguFalling.Invoke();
    }
}
