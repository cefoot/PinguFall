using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloeColor : MonoBehaviour
{
    public Color[] PossibleColors;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Renderer>().material.color = PossibleColors[Random.Range(0, PossibleColors.Length)];
    }

}
