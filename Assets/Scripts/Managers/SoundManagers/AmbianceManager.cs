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

    float fadeTime = 5f;


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
        audioSrc.volume = 0f;
    }

    Coroutine FadeIn;
    IEnumerator StartFadeIn()
    {

        while(audioSrc.volume < 1f)
        {
            audioSrc.volume += Time.deltaTime / fadeTime;
            yield return null;
        }
        audioSrc.volume = 1f;
        audioSrc.UnPause();
        FadeIn = null;
    }
    public void UnPauseAudio()
    {
        if (FadeIn != null) return;
        audioSrc.volume = 0f;
        FadeIn = StartCoroutine(StartFadeIn());
    }
}
