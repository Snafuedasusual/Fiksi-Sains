using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static IObjectiveSection;

public class DeadScientist : MonoBehaviour, IInteraction, IObjectiveSection
{
    [SerializeField] HandlerSection2 handle;

    [SerializeField] SectionEventComms sectionEventComms;
    [SerializeField] string objText;
    [SerializeField] IObjectiveSection.IsFinished currentStatus;
    [SerializeField] IObjectiveSection.IsLocked currentLockStatus;

    public void OnInteract(Transform plr)
    {
        if(currentLockStatus == IsLocked.Unlocked && currentStatus == IsFinished.NotDone)
        {
            OnDone();
            currentStatus = IsFinished.IsDone;
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
}
