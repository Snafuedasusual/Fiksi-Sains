using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadScientist : MonoBehaviour, F_Interaction
{
    [SerializeField] HandlerSection2 handle;
    public void OnInteract(Transform plr)
    {
        Debug.Log("On!");
        handle.CanFinish();
    }
}
