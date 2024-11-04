using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SA_VisualAudio : MonoBehaviour, IMakeSounds
{
    [SerializeField] private AudioSource audSrc;
    [SerializeField] private AudioClip genericSingleAudClip;

    public void GenericSingleAudioPlay()
    {
        if (genericSingleAudClip == null) return;
        RequestPlaySFXAudioClip(audSrc, genericSingleAudClip);
    }

    public void RequestPlaySFXAudioClip(AudioSource audSrc, AudioClip audClip)
    {
        SFXManager.instance.PlayAudio(audSrc, audClip);
    }
}
