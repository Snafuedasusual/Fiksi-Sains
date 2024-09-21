using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hide_Spots : MonoBehaviour, IInteraction
{
    public event EventHandler OnInteractActive;
    public event EventHandler OnInteractDeactive;
    public void OnInteract(Transform plr)
    {
        if(IsDebounce == null)
        {
            IsDebounce = Debounce();
            StartCoroutine(IsDebounce);
            Debug.Log("Hit!");
            if (plr.localScale != new Vector3(0.001f, 0.001f, 0.001f))
            {
                plr.localScale = new Vector3(0.001f, 0.001f, 0.001f);
                plr.GetComponent<PlayerLogic>().plrState = PlayerLogic.PlayerStates.Hiding;
            }
            else
            {
                plr.localScale = new Vector3(1f, 1f, 1f);
                plr.transform.position += transform.up + Vector3.up * 0.25f;
                plr.GetComponent<PlayerLogic>().plrState = PlayerLogic.PlayerStates.Idle;
            }
        }
        else
        {

        }
    }

    public void OnDetected(Transform plr)
    {

    }

    IEnumerator IsDebounce;


    IEnumerator Debounce()
    {
        var debTime = 0f;
        var debRate = 0.25f;
        while(debTime < debRate)
        {
            debTime += Time.deltaTime;
            yield return 0;
        }
        IsDebounce = null;
    }
}
