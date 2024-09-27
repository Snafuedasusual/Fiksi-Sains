using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static IObjectiveSection;

public class DeadScientist : MonoBehaviour, IInteraction, IObjectiveSection
{
    [SerializeField] HandlerSection2 handle;

    [SerializeField] SectionEventComms sectionEventComms;
    [SerializeField] string objText;
    [SerializeField] string notif;
    [SerializeField] IObjectiveSection.IsFinished currentStatus;
    [SerializeField] IObjectiveSection.IsLocked currentLockStatus;
    [SerializeField] RuntimeAnimatorController controller;

    private float currentProgress = 0f;
    private float maxProgress = 100f;

    public event EventHandler OnInteractActive;
    public event EventHandler OnInteractDeactive;

    private PlayerInput plrInp;
    private PlayerLogic plrLogic;


    Coroutine CutHand;
    IEnumerator StartCutHand()
    {
        if (plrLogic != null) { plrLogic.PlaySpecialActor(controller); }
        WaitBarManager.instance.ActivateWaitBar();
        WaitBarManager.instance.UpdateBarValue(0);
        var timer = 0f;
        var maxTimer = 0.075f;
        while(currentProgress < maxProgress)
        {
            timer = 0f;
            while(timer < maxTimer)
            {
                if (plrInp.GetInputDir() != Vector2.zero) { StopCoroutine(CutHand); CutHand = null; WaitBarManager.instance.DeactivateWaitBar(); plrLogic.DisableSpecialActor(); yield break; }
                timer += Time.deltaTime;
                yield return null;
            }
            currentProgress++;
            WaitBarManager.instance.UpdateBarValue(currentProgress);
        }
        if (currentProgress >= maxProgress) { OnDone(); currentStatus = IsFinished.IsDone; CutHand = null; }
        if (plrLogic != null) { plrLogic.plrState = PlayerLogic.PlayerStates.Idle; plrLogic.DisableSpecialActor(); }
        WaitBarManager.instance.DeactivateWaitBar();
        CutHand = null;

    }
    Coroutine InteractDebounce;
    IEnumerator StartInteractDebounce()
    {
        var timer = 0f;
        var maxTimer = 0.1f;
        while (timer < maxTimer)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        InteractDebounce = null;
    }
    public void OnInteract(Transform plr)
    {
        if (currentLockStatus == IsLocked.Locked || currentStatus == IsFinished.IsDone) return;
        if (InteractDebounce != null) return;
        InteractDebounce = StartCoroutine(StartInteractDebounce());
        if (CutHand != null) 
        { 
            StopCoroutine(CutHand); 
            CutHand = null; 
            WaitBarManager.instance.DeactivateWaitBar();
            if (plrLogic != null)  { plrLogic.plrState = PlayerLogic.PlayerStates.Idle; plrLogic.DisableSpecialActor(); } 
        }
        else 
        { 
            plrInp = plr.TryGetComponent(out PlayerInput inp) ? inp : null;
            plrLogic = plr.TryGetComponent(out PlayerLogic lgc) ? lgc : null;
            if (plrInp == null) return;
            if (plrLogic == null) return;
            plrLogic.plrState = PlayerLogic.PlayerStates.InteractingHold;
            CutHand = StartCoroutine(StartCutHand()); 
            Debug.Log("Haven't played"); 
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

    public string UpdateNotif()
    {
        return notif; 
    }
}
