using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlrSoundController : MonoBehaviour, IMakeSounds
{
    [Header("Script References")]
    [SerializeField] PlayerLogic plrLogic;

    [Header("Variables")]
    [SerializeField] float soundBar = 0;
    [SerializeField] LayerMask characters;
    [SerializeField] Collider[] soundListeners;


    private void Start()
    {
        plrLogic.ProduceSound += ProduceSoundReceiver;
    }

    private void ProduceSoundReceiver(object sender, PlayerLogic.ProduceSoundArgs e)
    {
        SoundProducer(e.soundSize);
        SoundBarUpdater(e.soundSize);
    }

    private void SoundBarUpdater(float value)
    {
        if(value == 0)
        {
            soundBar = 0;
            return;
        }
        if(soundBar > 0)
        {
            if(value > soundBar)
            {
                soundBar = value;
                return;
            }
            else
            {

            }
        }
        else if(soundBar == 0)
        {
            soundBar = value;
        }
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
