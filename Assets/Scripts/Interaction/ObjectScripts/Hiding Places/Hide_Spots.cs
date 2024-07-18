using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hide_Spots : MonoBehaviour, IInteraction
{
    public void OnInteract(Transform plr)
    {
        Debug.Log("Hit!");
        if(plr.localScale != new Vector3(0.001f, 0.001f, 0.001f))
        {
            plr.localScale = new Vector3(0.001f, 0.001f, 0.001f);
            plr.GetComponent<PlayerLogic>().plrState = PlayerLogic.PlayerStates.Hiding;
        }
        else
        {
            plr.localScale = new Vector3(1f, 1f, 1f);
            plr.GetComponent<PlayerLogic>().plrState = PlayerLogic.PlayerStates.Idle;
        }
    }
}
