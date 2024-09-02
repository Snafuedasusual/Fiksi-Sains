using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbianceManager : MonoBehaviour
{
    public static AmbianceManager instance;
    [SerializeField] AudioSource audioSrc;
    [SerializeField] AmbianceSO ambianceSO;

    private AudioClip[] ambianceClips;
    private int[] selectedClips;

    private void Awake()
    {
        if(instance != null && instance != this)
        {

        }
        else
        {
            instance = this;
        }
    }

    private void Start()
    {
        ambianceClips = ambianceSO.audioClips;
    }

    public void RequestPlay(int[] clips)
    {
        selectedClips = new int[clips.Length];
        for (int i = 0;  i < selectedClips.Length; i++)
        {
            selectedClips[i] = clips[i];
        }
        PlayAudio();
    }

    public void RefreshAudio()
    {
        audioSrc.Stop();
        selectedClips = new int[0];
    }

    public void PlayAudio()
    {
        audioSrc.clip = ambianceClips[selectedClips[Random.Range(0, selectedClips.Length)]];
        audioSrc.loop = true;
        audioSrc.Play();
    }

    public void PauseAudio()
    {
        audioSrc.Pause();
    }

    public void UnPauseAudio()
    {
        audioSrc.UnPause();
    }
}
