using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Hide_Spots : MonoBehaviour, IInteraction, IMakeSounds
{
    [SerializeField] string notif;
    [SerializeField] AudioSource audSrc;
    [SerializeField] AudioClip[] audClips;
    [SerializeField] TextAsset subtitle;

    public event EventHandler OnInteractActive;
    public event EventHandler OnInteractDeactive;
    private bool entered = false;
    public void OnInteract(Transform plr)
    {
        if (!plr.TryGetComponent(out PlayerLogic lgc)) return;
        if (lgc.GetSeenStatus() == EntitySeenStatus.Seen) { SubtitleManager.instance.ActivateSubtitle(subtitle); return; }
        {
            
        }
        if (IsDebounce == null)
        {
            IsDebounce = Debounce();
            StartCoroutine(IsDebounce);
            if (entered == false)
            {
                entered = true;
                plr.GetComponent<PlayerLogic>().plrState = PlayerLogic.PlayerStates.Hiding;
                plr.GetComponent<PlayerLogic>().HidePlayer();
            }
            else
            {
                entered = false;
                plr.GetComponent<PlayerLogic>().UnHidePlayer();
                plr.transform.position += transform.up + Vector3.up * 0.25f;
                if (Physics.Raycast(plr.transform.position, -Vector3.up, out RaycastHit hit)) plr.transform.position = hit.point; 
                plr.GetComponent<PlayerLogic>().plrState = PlayerLogic.PlayerStates.Idle;
            }
            PlayAudioClip();
        }
        else
        {

        }
    }

    public void OnDetected(Transform plr)
    {

    }

    IEnumerator IsDebounce;



    IEnumerator Debounce()
    {
        var debTime = 0f;
        var debRate = 0.25f;
        while(debTime < debRate)
        {
            debTime += Time.deltaTime;
            yield return 0;
        }
        IsDebounce = null;
    }

    private void PlayAudioClip()
    {
        if (audSrc == null) return;
        if (audClips.Length <= 0) return;
        if (audClips.Length == 1) RequestPlaySFXAudioClip(audSrc, audClips[0]);
        else
        {
            var selectedAudioClip = Random.Range(0, audClips.Length);
            RequestPlaySFXAudioClip(audSrc, audClips[selectedAudioClip]);
        }
    }

    public void RequestPlaySFXAudioClip(AudioSource audSrc, AudioClip audClip)
    {
        SFXManager.instance.PlayAudio(audSrc, audClip);
    }

    public string UpdateNotif()
    {
        return notif;
    }
}
