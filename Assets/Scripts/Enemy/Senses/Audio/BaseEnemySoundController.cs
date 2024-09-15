using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class BaseEnemySoundController : MonoBehaviour, IMakeSounds
{
    [SerializeField] BaseEnemyStatsSO statsSO;
    [SerializeField] EntityAudioClipsSO entityAudioClips;

    [SerializeField] AudioSource audSrc;
    [SerializeField] BaseEnemyLogic enmyLgc;

    [Header("Variables")]
    [SerializeField] float hearingModifier;


    private void OnEnable()
    {
        hearingModifier = statsSO.hearingModifier;
        enmyLgc.PlayIdleAudioOnUpdateEvent += PlayIdleAudioOnUpdateEventReceiver;
    }


    public void SoundProducer(float soundAdder)
    {
        
    }

    public event EventHandler<SoundToLogicArgs> SoundToLogic;
    public class SoundToLogicArgs : EventArgs 
    { public float alertLevel; public Vector3 location; }


    public void SoundReceiver(Vector3 soundSrc, float soundSize)
    {
        var lowAlertLevel = 1f;
        var highAlertLevel = 2f;
        var distance = Vector3.Distance(soundSrc, transform.position);
        if(distance < (soundSize * hearingModifier) && distance > (soundSize * hearingModifier) * 0.75f)
        {
            SoundToLogic?.Invoke(this, new SoundToLogicArgs { alertLevel = lowAlertLevel, location = soundSrc });
        }
        else if(distance < (soundSize * hearingModifier) * 0.75f)
        {
            SoundToLogic?.Invoke(this, new SoundToLogicArgs { alertLevel = highAlertLevel, location = soundSrc });
        }
        else
        {

        }
    }


    private void PlayIdleAudioOnUpdateEventReceiver(object sender, EventArgs e)
    {
        if (entityAudioClips == null) return;
        if (entityAudioClips.idleClips.Length <= 0) return;
        var audioToChoose = entityAudioClips.idleClips[Random.Range(0, entityAudioClips.idleClips.Length)];
        SFXManager.instance.PlayAudio(audSrc, audioToChoose);
    }


    public void RequestPlayAudioClip(AudioSource audSrc, AudioClip audClip)
    {

    }
}
