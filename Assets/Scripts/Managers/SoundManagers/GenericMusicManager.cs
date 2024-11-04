using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericMusicManager : MonoBehaviour
{
    public static GenericMusicManager instance;

    [SerializeField] AudioSource audSrc;

    float fadeTime = 5f;
    float originalVolume = 0.7f;

    private void Awake()
    {
        if (instance != null && instance != this) return;
        if (instance == null) instance = this;
    }


    public void PlayGenericMusic(AudioClip music)
    {
        if (music == null) return;
        if (music == audSrc.clip && audSrc.isPlaying == true) return;
        AmbianceManager.instance.PauseAudio();
        audSrc.volume = originalVolume;
        audSrc.clip = music;
        audSrc.Play();
    }

    Coroutine FadeOut;
    IEnumerator StartFadeOut()
    {
        while (audSrc.volume > 0.01f)
        {
            audSrc.volume -= Time.deltaTime / fadeTime;
            yield return null;
        }
        audSrc.volume = 0;
        audSrc.clip = null;
        audSrc.Stop();
        FadeOut = null;
    }

    public void StopMusic()
    {
        if (FadeOut != null) return;
        FadeOut = StartCoroutine(StartFadeOut());
        AmbianceManager.instance.UnPauseAudio();
    }
}
