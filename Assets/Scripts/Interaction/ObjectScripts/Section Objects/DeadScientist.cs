using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadScientist : MonoBehaviour, IInteraction
{
    [SerializeField] HandlerSection2 handle;
    public void OnInteract(Transform plr)
    {
        Debug.Log("On!");
        handle.CanFinish();
    }
}
