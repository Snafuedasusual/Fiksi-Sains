using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Levers : MonoBehaviour, IInteraction
{
    [SerializeField] HandlerSection3 handler;
    public void OnInteract(Transform plr)
    {
        handler.InteractedLever();
    }
    
}
