using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractSubtitleTrigger : MonoBehaviour, IInteraction
{
    [SerializeField] TextAsset dialogue;
    public event EventHandler OnInteractActive;
    public event EventHandler OnInteractDeactive;

    public void OnDetected(Transform plr)
    {
        throw new System.NotImplementedException();
    }

    Coroutine IsInteractDebounce;


    IEnumerator StartInteractDebounce()
    {
        var timePlay = 0f;
        var maxTime = 0.15f;
        while(timePlay < maxTime)
        {
            timePlay += Time.deltaTime;
            yield return null;
        }
        IsInteractDebounce = null;
    }
    public void OnInteract(Transform plr)
    {
        if (IsInteractDebounce != null) return;
        IsInteractDebounce = StartCoroutine(StartInteractDebounce());
        SubtitleManager.instance.ActivateSubtitle(dialogue);
    }
}
