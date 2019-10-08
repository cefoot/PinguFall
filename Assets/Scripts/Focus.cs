using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Focus : MonoBehaviour
{
    private Transform _transform;
    public Transform Target;

    // Start is called before the first frame update
    void Start()
    {
        _transform = GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        _transform.rotation = Quaternion.Lerp(_transform.rotation, Quaternion.LookRotation(Target.position - _transform.position, Vector3.up), Time.deltaTime);
    }
}
