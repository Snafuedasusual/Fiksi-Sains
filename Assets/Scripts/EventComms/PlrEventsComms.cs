using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlrEventsComms : MonoBehaviour
{
    public event EventHandler PlayFootstepAudioEvent;
    public void PlayFootstepAudio()
    {
        PlayFootstepAudioEvent?.Invoke(this, EventArgs.Empty);    
    }
}
