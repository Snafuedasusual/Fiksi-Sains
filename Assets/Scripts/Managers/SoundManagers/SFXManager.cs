using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXManager : MonoBehaviour
{
    public static SFXManager instance;

    private void Awake()
    {
        if (instance != null && instance != this) return;
        if (instance == null) instance = this;
    }

    public void PlayAudio(AudioSource audSrc, AudioClip audClip)
    {
        if(audSrc.clip != null) audSrc.Stop();
        audSrc.clip = audClip;
        audSrc.Play();
    }

    public void StopAudio(AudioSource audSrc)
    {
        audSrc.Stop();
        audSrc.clip = null;
    }
}
