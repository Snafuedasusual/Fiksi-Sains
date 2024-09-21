using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interact_Obj : MonoBehaviour, IInteraction
{
    public event EventHandler OnInteractActive;
    public event EventHandler OnInteractDeactive;

    public void OnInteract(Transform plr)
    {
        Debug.Log("Its an Object!");
    }

    public void OnDetected(Transform plr)
    {

    }
}
