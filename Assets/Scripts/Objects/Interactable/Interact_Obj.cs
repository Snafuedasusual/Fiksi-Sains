using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interact_Obj : MonoBehaviour, F_Interaction
{
    public void OnInteract(Transform plr)
    {
        Debug.Log("Its an Object!");
    }
}
