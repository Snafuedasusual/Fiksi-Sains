using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static IObjectiveSection;

public class Door_Section : MonoBehaviour, IInteraction, IObjectiveSection, IMakeSounds
{
    [SerializeField] SectionEventComms sectionEventComms;

    [SerializeField] string objText;
    [SerializeField] string notif;
    [SerializeField] TextAsset ifLockedText;
    [SerializeField] AudioSource audSrc;
    [SerializeField] AudioClip lockedSound;
    [SerializeField] IObjectiveSection.IsFinished currentStatus;
    [SerializeField] IObjectiveSection.IsLocked currentLockStatus;
    [SerializeField] IObjectiveSection.HasIndicator canIndicate;

    public event EventHandler OnInteractActive;
    public event EventHandler OnInteractDeactive;

    public void OnInteract(Transform plr)
    {
        if (currentLockStatus == IObjectiveSection.IsLocked.Unlocked && currentStatus == IsFinished.NotDone)
        {
            OnDone();
            currentStatus = IsFinished.IsDone;
        }
        else
        {
            if(ifLockedText != null) { SubtitleManager.instance.ActivateSubtitle(ifLockedText); RequestPlaySFXAudioClip(audSrc, lockedSound); }
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
        currentLockStatus = currentLockStatus = IsLocked.Locked;
    }

    public void OnDone()
    {
        sectionEventComms.OnObjectiveDone(gameObject);
    }

    public void ResetObj()
    {
        currentStatus = IsFinished.NotDone;
        currentLockStatus = IsLocked.Locked;
    }

    public IsLocked LockOrUnlocked()
    {
        return currentLockStatus;
    }

    public string GetObjText()
    {
        return objText;
    }

    public void ForceDone()
    {
        currentLockStatus = IsLocked.Unlocked;
        currentStatus = IsFinished.IsDone;
    }

    public string UpdateNotif()
    {
        return notif; 
    }

    public void RequestPlaySFXAudioClip(AudioSource audSrc, AudioClip audClip)
    {
        SFXManager.instance.PlayAudio(audSrc, audClip);
    }

    public HasIndicator CanHaveIndicator()
    {
        return canIndicate;
    }
}
