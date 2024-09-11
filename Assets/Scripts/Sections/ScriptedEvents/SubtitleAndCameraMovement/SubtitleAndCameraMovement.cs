using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubtitleAndCameraMovement : MonoBehaviour, IScriptedEvents
{
    [SerializeField]private IScriptedEvents.Triggered isTriggered;
    [SerializeField] private CinemachineVirtualCamera cinemachineCam;
    [SerializeField] private TextAsset dialogue;
    private PlayerLogic playerLogic;

    public void ResetTrigger()
    {
        isTriggered = IScriptedEvents.Triggered.NotTriggered;
    }

    Coroutine StartCamMovement;
    IEnumerator CameMovementStart()
    {
        var playTime = 0f;
        var playRate = 3.25f;
        while(playTime < playRate)
        {
            playTime += Time.deltaTime;
            yield return null;
        }
        StartCamMovement = null;
        cinemachineCam.Priority = 1;
        playerLogic.UnNullifyState();
    }
    public void Trigger()
    {
        if (isTriggered == IScriptedEvents.Triggered.HasTriggered) return;
        cinemachineCam.Priority = 11;
        playerLogic.NullifyState();
        StartCamMovement = StartCoroutine(CameMovementStart());
        isTriggered = IScriptedEvents.Triggered.HasTriggered;
        if (dialogue == null) return;
        SubtitleManager.instance.ActivateSubtitle(dialogue);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out PlayerLogic plrLgc))
        {
            playerLogic = plrLgc;
            Trigger();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        playerLogic = null;
    }
}
