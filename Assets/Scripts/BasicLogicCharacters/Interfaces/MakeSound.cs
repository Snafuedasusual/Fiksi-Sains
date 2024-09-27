using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface IMakeSounds
{
    void SoundProducer(float soundAdder)
    {

    }
    void SoundReceiver(Vector3 soundSrc, float soundSize)
    {


    }

    public void RequestPlayAudioClip(AudioSource audSrc, AudioClip audClip);

    public void RequestStopAudioSource(AudioSource audSrc)
    {

    }
}
