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

    public void RequestPlaySFXAudioClip(AudioSource audSrc, AudioClip audClip);

    public void RequestStopAudioSource(AudioSource audSrc)
    {

    }

    public void RequestPlayGenericMusicAudioClip(AudioClip audClip)
    {

    }

     public void RequestStopGenericMusicAudioClip()
    {

    }
}
