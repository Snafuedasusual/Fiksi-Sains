using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static IObjectiveSection;

public class Activation_Object : MonoBehaviour, IInteraction, IObjectiveSection, IMakeSounds
{
    [SerializeField] HandlerSection1 handler;

    [SerializeField] SectionEventComms sectionEventComms;
    [SerializeField] string notif;
    [SerializeField] string objText;
    [SerializeField] AudioSource audSrc;
    [SerializeField] AudioClip audioClip;
    [SerializeField] IObjectiveSection.IsFinished currentStatus;
    [SerializeField] IObjectiveSection.IsLocked currentLockStatus;
    [SerializeField] IObjectiveSection.HasIndicator canIndicate; 

    public event EventHandler OnInteractActive;
    public event EventHandler OnInteractDeactive;

    Coroutine InteractDebounce;
    IEnumerator StartInteractDebounce()
    {
        var debTime = 0f;
        var debTimeMax = 0.1f;
        while(debTime < debTimeMax)
        {
            debTime += Time.deltaTime;
            yield return null;
        }
        InteractDebounce = null;
    }
    public void OnInteract(Transform plr)
    {
        if (InteractDebounce != null) return;
        if (currentLockStatus == IObjectiveSection.IsLocked.Unlocked && currentStatus == IsFinished.NotDone)
        {
            OnDone();
            currentStatus = IsFinished.IsDone;
            InteractDebounce = StartCoroutine(StartInteractDebounce());
        }
        else
        {

        }
    }

    public void OnDetected(Transform plr)
    {

    }

    public void Unlocked()
    {
        currentLockStatus = IsLocked.Unlocked;
    }

    public void Lock()
    {
        currentLockStatus = IsLocked.Locked;
    }

    public void OnDone()
    {
        sectionEventComms.OnObjectiveDone(gameObject);
        RequestPlaySFXAudioClip(audSrc, audioClip);
    }

    public void ResetObj()
    {
        currentStatus = IsFinished.NotDone;
        currentLockStatus = IsLocked.Locked;
    }

    public IsLocked LockOrUnlocked()
    {
        return IsLocked.Locked;
    }

    public string GetObjText()
    {
        return objText;
    }

    public void ForceDone()
    {

    }

    public string UpdateNotif()
    {
        return notif;
    }

    public void RequestPlaySFXAudioClip(AudioSource audSrc, AudioClip audClip)
    {
        if (audSrc == null) return;
        if (audClip == null) return;
        SFXManager.instance.PlayAudio(audSrc, audClip);
    }

    public HasIndicator CanHaveIndicator()
    {
        return canIndicate;
    }
}
