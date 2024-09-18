using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static IObjectiveSection;

public class Ending : MonoBehaviour, IObjectiveSection
{
    [SerializeField] SectionEventComms sectionEventComms;

    [SerializeField] string objText;
    [SerializeField] TextAsset ifLockedText;
    [SerializeField] IObjectiveSection.IsFinished currentStatus;
    [SerializeField] IObjectiveSection.IsLocked currentLockStatus;

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

    Coroutine PlayScriptedEvent;
    IEnumerator StartPlayScriptedEvent()
    {
        var playTime = 0f;
        var playTimeMax = 7.5f;
        while (playTime < playTimeMax)
        {
            playTime += Time.deltaTime;
            yield return null;
        }
        PlayScriptedEvent = null;
        OnDone();
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out PlayerLogic plrLgc))
        {
            if (currentLockStatus == IsLocked.Locked) return;
            if (currentStatus == IsFinished.IsDone) return;
            PlayScriptedEvent = StartCoroutine(StartPlayScriptedEvent());
            sectionEventComms.StartScriptedEventNoArgs();
        }
    }
}
