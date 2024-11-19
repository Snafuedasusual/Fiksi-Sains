using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S1_EngConv : MonoBehaviour, IScriptedEvents
{
    private IScriptedEvents.Triggered isTriggered = IScriptedEvents.Triggered.NotTriggered;
    [SerializeField] TextAsset dialogue;
    [SerializeField] CinemachineVirtualCamera vCam;
    [SerializeField] Transform actor;
    [SerializeField] Animator animator;
    [SerializeField] RuntimeAnimatorController SA_idleController;
    [SerializeField] RuntimeAnimatorController SA_suicideController;

    private Transform player;
    private PlayerLogic playerLogic;

    public void ResetTrigger()
    {
        isTriggered = IScriptedEvents.Triggered.NotTriggered;
    }

    Coroutine CheckIfDialogueFinished;
    IEnumerator CheckDialogueFinish()
    {
        var timer = 0f;
        var maxTimer = 0.2f;
        while (timer < maxTimer)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        while(SubtitleManager.instance.GetIfDialogueFinished() == false)
        {
            if (SubtitleManager.instance.GetIfDialogueFinished() == true) { break; }
            yield return null;
        }
        CheckIfDialogueFinished = null;
        AnimationPlay();
    }


    private void AnimationPlay()
    {
        if (AnimationPlayOut != null) return;
        AnimationPlayOut = StartCoroutine(StartAnimationPlayOut());
    }
    Coroutine AnimationPlayOut;
    IEnumerator StartAnimationPlayOut()
    {
        animator.runtimeAnimatorController = SA_suicideController;
        actor.gameObject.SetActive(false);
        actor.gameObject.SetActive(true);
        var timer = 0f;
        var maxTimer = 2.5f;
        while (timer < maxTimer)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        AnimationPlayOut = null;
        vCam.Priority = 1;
        playerLogic.UnNullifyState();
    }

    public void Trigger()
    {
        if (isTriggered == IScriptedEvents.Triggered.HasTriggered) return;
        isTriggered = IScriptedEvents.Triggered.HasTriggered;
        if (dialogue == null) return;
        SubtitleManager.instance.ActivateSubtitle(dialogue);
        vCam.Priority = 11;
        playerLogic.NullifyState();
        player.transform.LookAt(new Vector3(actor.position.x, player.position.y, actor.position.z));
        CheckIfDialogueFinished = StartCoroutine(CheckDialogueFinish());

    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out PlayerLogic plrLogic))
        {
            playerLogic = plrLogic;
            player = other.transform;
            Trigger();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out PlayerLogic plrLogic))
        {
            playerLogic = null;
            player = null;
        }
    }
}
