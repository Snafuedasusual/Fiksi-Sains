using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static IObjectiveSection;

public class Door_Section : MonoBehaviour, IInteraction, IObjectiveSection
{
    [SerializeField] SectionEventComms sectionEventComms;

    [SerializeField] string objText;
    [SerializeField] TextAsset ifLockedText;
    [SerializeField] IObjectiveSection.IsFinished currentStatus;
    [SerializeField] IObjectiveSection.IsLocked currentLockStatus;
    public void OnInteract(Transform plr)
    {
        if (currentLockStatus == IObjectiveSection.IsLocked.Unlocked && currentStatus == IsFinished.NotDone)
        {
            OnDone();
            currentStatus = IsFinished.IsDone;
        }
        else
        {
            if(ifLockedText != null) { SubtitleManager.instance.ActivateSubtitle(ifLockedText); }
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

    }
}
