using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LittleVent : MonoBehaviour, IInteraction
{
    [SerializeField] Transform plrSpot;
    [SerializeField] BrainVent brainVent;

    public void OnInteract(Transform plr)
    {
        brainVent.InitializeVent(plrSpot, plr);
    }
}
