using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LittleVent : MonoBehaviour, IInteraction
{
    [SerializeField] Transform plrSpot;
    [SerializeField] BrainVent brainVent;

    [SerializeField] string notif;

    public event EventHandler OnInteractActive;
    public event EventHandler OnInteractDeactive;

    public void OnInteract(Transform plr)
    {
        if (IsDebounce == null)
        {
            IsDebounce = Debounce();
            StartCoroutine(IsDebounce);
            brainVent.InitializeVent(plrSpot, plr);
        }
    }

    public void OnDetected(Transform plr)
    {

    }

    IEnumerator IsDebounce;

    IEnumerator Debounce()
    {
        var debTime = 0f;
        var debRate = 0.15f;
        while(debTime < debRate)
        {
            debTime += Time.deltaTime;
            yield return 0;
        }
        IsDebounce = null;
    }

    public string UpdateNotif()
    {
        return notif;
    }
}
