using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SubtitleTrigger : MonoBehaviour, IScriptedEvents
{
    [SerializeField] TextAsset triggeredDialogue;

    private IScriptedEvents.Triggered isTriggered = IScriptedEvents.Triggered.NotTriggered;
    public void ResetTrigger()
    {
        isTriggered = IScriptedEvents.Triggered.NotTriggered;
    }

    public void Trigger()
    {
        if(isTriggered == IScriptedEvents.Triggered.NotTriggered)
        {
            SubtitleManager.instance.ActivateSubtitle(triggeredDialogue);
            isTriggered = IScriptedEvents.Triggered.HasTriggered;
        }
        else
        {
            return;
        }
    }

    private void OnTriggerEnter()
    {
        Trigger();
    }
}
