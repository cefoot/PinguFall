using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseInput : MonoBehaviour
{

    public GameObject Particle;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var hit))
            {
                hit.collider.SendMessageUpwards("Drop", SendMessageOptions.DontRequireReceiver);
                if (Particle) Instantiate(Particle, hit.point, Quaternion.identity);

            }
        }
    }
}
