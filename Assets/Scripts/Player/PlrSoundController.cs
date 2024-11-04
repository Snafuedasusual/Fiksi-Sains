using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlrSoundController : MonoBehaviour, IMakeSounds, IInitializeScript
{
    [Header("Script References")]
    [SerializeField] PlayerLogic plrLogic;

    [Header("Components")]
    [SerializeField] AudioSource audSrc;

    [Header("Variables")]
    [SerializeField] float soundBar = 0;
    [SerializeField] LayerMask characters;
    [SerializeField] Collider[] soundListeners;
    [SerializeField] EntityAudioClipsSO entityClipsSO;

    public void InitializeScript()
    {
        plrLogic.ProduceSound += ProduceSoundReceiver;
        plrLogic.PlayAudioClip += PlayAudioClipReceiver;
    }


    public void DeInitializeScript()
    {
        plrLogic.ProduceSound -= ProduceSoundReceiver;
        plrLogic.PlayAudioClip -= PlayAudioClipReceiver;
    }

    private void OnEnable()
    {
        InitializeScript();
    }

    private void OnDisable()
    {
        DeInitializeScript();
    }

    private void OnDestroy()
    {
        DeInitializeScript();
    }

    private void PlayAudioClipReceiver(object sender, PlayerLogic.PlayAudioClipEventArgs e)
    {
        if(e.audioType == EntityAudioClipsSO.AudioTypes.Pickup)
        {
            if (entityClipsSO == null) return;
            if (entityClipsSO.pickupClips.Length <= 0) return;
            if (entityClipsSO.pickupClips.Length == 1) RequestPlaySFXAudioClip(audSrc, entityClipsSO.pickupClips[0]);
            else
            {
                var selectedAudioClip = Random.Range(0, entityClipsSO.pickupClips.Length);
                RequestPlaySFXAudioClip(audSrc, entityClipsSO.pickupClips[selectedAudioClip]);
            }
        }
    }

    public void RequestPlaySFXAudioClip(AudioSource audSrc, AudioClip audClip)
    {
        SFXManager.instance.PlayAudio(audSrc, audClip);
    }


    private void ProduceSoundReceiver(object sender, PlayerLogic.ProduceSoundArgs e)
    {
        SoundProducer(e.soundSize);
        SoundBarUpdater(e.soundSize);
    }

    private void SoundBarUpdater(float value)
    {
        soundBar = value;
    }


    public void SoundProducer(float soundAdder)
    {
        soundListeners = Physics.OverlapSphere(transform.position, 100f, characters);
        for(int i = 0; i < soundListeners.Length; i++)
        {
            if (soundListeners[i].transform.TryGetComponent(out IMakeSounds listener))
            {
                if (soundListeners[i].TryGetComponent(out BaseEnemyLogic enemy))
                {
                    listener.SoundReceiver(transform.position, soundAdder);
                }
            }
        }
    }
}
