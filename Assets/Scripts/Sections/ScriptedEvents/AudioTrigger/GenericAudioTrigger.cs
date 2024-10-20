using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericAudioTrigger : MonoBehaviour, IScriptedEvents, IMakeSounds
{
    [SerializeField] AudioSource audSrc;
    [SerializeField] AudioClip audClip;
    [SerializeField] TextAsset dialogue;
    [SerializeField] float delayTime;
    [SerializeField] IScriptedEvents.Triggered isTriggered;

    public void RequestPlaySFXAudioClip(AudioSource audSrc, AudioClip audClip)
    {
        SFXManager.instance.PlayAudio(audSrc, audClip);
    }

    public void ResetTrigger()
    {
        isTriggered = IScriptedEvents.Triggered.NotTriggered;
    }


    Coroutine DelayTime;
    IEnumerator StartDelayTime()
    {
        var timer = 0f;
        var maxTimer = delayTime;
        while(timer < maxTimer)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        if (dialogue == null) { DelayTime = null; yield break; }
        SubtitleManager.instance.ActivateSubtitle(dialogue);
    }

    public void Trigger()
    {
        if (isTriggered == IScriptedEvents.Triggered.HasTriggered) return;
        if (DelayTime != null) return;
        DelayTime = StartCoroutine(StartDelayTime());
        isTriggered = IScriptedEvents.Triggered.HasTriggered;
        RequestPlaySFXAudioClip(audSrc, audClip);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent(out PlayerLogic Lgc)) return;
        Trigger();
    }
}
