using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MenuAction : MonoBehaviour
{

    public UnityEvent ButtonClicked;

    public void Drop()
    {
        ButtonClicked.Invoke();
    }

}
