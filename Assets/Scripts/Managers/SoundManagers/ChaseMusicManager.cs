using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseMusicManager : MonoBehaviour
{
    public static ChaseMusicManager instance;
    private void Awake()
    {
        if (instance != null && instance != this) return;
        if (instance == null) instance = this;
    }

    private bool isChaseMusic = false;

    [SerializeField] AudioSource audSrc;
    [SerializeField] MusicSO musicSO;
    [SerializeField] List<GameObject> enemiesAlert = new List<GameObject>();

    private float fadeTime = 5f;

    public void AddEnemyAlert(GameObject enemy)
    {
        if (enemiesAlert.Count <= 0) { enemiesAlert.Add(enemy); PlayChaseMusic(); }
        else
        {
            for (int i = 0; i < enemiesAlert.Count; i++)
            {
                if (enemiesAlert[i] == enemy) break;
                else if (i == enemiesAlert.Count - 1 && enemiesAlert[i] != enemy)
                {
                    enemiesAlert.Add(enemy);
                }
            }

        }

    }

    public void RemoveEnemyAlert(GameObject enemy)
    {
        for(int i = 0; i < enemiesAlert.Count; i++)
        {
            if (enemiesAlert[i] == enemy)  { enemiesAlert.Remove(enemy); break; }
        }
        if (enemiesAlert.Count <= 0) FadeOutChase();
    }

    public void PlayChaseMusic()
    {
        var handler = FindObjectOfType<BaseHandler>();
        if (handler == null) return;
        if (handler.GetChaseMusicClip() >= 1000) return;
        if (musicSO == null) return;
        if (musicSO.chaseMusic.Length <= 0) return;
        if (musicSO.chaseMusic.Length == 1 && handler.GetChaseMusicClip() == 0) 
        {
            if (FadeOutChaseMusic != null) return;
            if (audSrc.volume < 1f) audSrc.volume = 1f;
            if (isChaseMusic == true) return;
            audSrc.clip = musicSO.chaseMusic[0]; 
            audSrc.Play();
            audSrc.loop = true;
            isChaseMusic = true;
            return; 
        }
        else 
        {
            if(FadeOutChaseMusic != null) return;
            if (audSrc.volume < 1f) audSrc.volume = 1f;
            if (isChaseMusic == true) return;
            audSrc.clip = musicSO.chaseMusic[handler.GetChaseMusicClip()]; 
            audSrc.Play(); 
            audSrc.loop = true; 
            isChaseMusic = true;
            return; 
        }
    }

    Coroutine FadeOutChaseMusic;
    IEnumerator StartFadeOutMusic()
    {
        if (audSrc.clip != null && isChaseMusic == false) { FadeOutChaseMusic = null; yield break; }
        while(audSrc.volume > 0.01f)
        {
            audSrc.volume -= Time.deltaTime / fadeTime;
            yield return null;
        }
        audSrc.volume = 0;
        audSrc.Stop();
        audSrc.clip = null;
        isChaseMusic = false;
        FadeOutChaseMusic = null;
    }


    private void FadeOutChase()
    {
        if (FadeOutChaseMusic != null) return;
        FadeOutChaseMusic = StartCoroutine(StartFadeOutMusic());
    }

    public void StopMusic()
    {
        audSrc.Stop();
        audSrc.clip = null;
        isChaseMusic = false;
        enemiesAlert.Clear();
    }
}
