using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Activation_Object : MonoBehaviour, F_Interaction
{
    [SerializeField] HandlerSection1 handler;
    public void OnInteract(Transform plr)
    {
        Debug.Log("On!");
        handler.UnlockDoorSection();
    }
}
