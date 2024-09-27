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

    public event EventHandler OnInteractActive;
    public event EventHandler OnInteractDeactive;
    private bool entered = false;
    public void OnInteract(Transform plr)
    {
        if(IsDebounce == null)
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
        if (audClips.Length == 1) RequestPlayAudioClip(audSrc, audClips[0]);
        else
        {
            var selectedAudioClip = Random.Range(0, audClips.Length);
            RequestPlayAudioClip(audSrc, audClips[selectedAudioClip]);
        }
    }

    public void RequestPlayAudioClip(AudioSource audSrc, AudioClip audClip)
    {
        SFXManager.instance.PlayAudio(audSrc, audClip);
    }

    public string UpdateNotif()
    {
        return notif;
    }
}
