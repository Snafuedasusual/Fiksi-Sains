using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlrSoundController : MonoBehaviour, IMakeSound
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
        soundBar = value;
    }


    public void SoundProducer(float soundAdder)
    {
        soundListeners = Physics.OverlapSphere(transform.position, 100f, characters);
        for(int i = 0; i < soundListeners.Length; i++)
        {
            if (soundListeners[i].transform.TryGetComponent(out IMakeSound listener))
            {
                if (soundListeners[i].TryGetComponent(out BaseEnemyLogic enemy))
                {
                    listener.SoundReceiver(transform.position, soundAdder);
                }
            }
        }
    }
}
