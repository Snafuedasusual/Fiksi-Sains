using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PodControl : MonoBehaviour, IInteraction
{
    [SerializeField] FinalHandler handler;
    public void OnInteract(Transform plr)
    {
        Debug.Log("Hit!");
        handler.ActivateEnd();
    }
}
