using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventCommsUI : MonoBehaviour
{
    public event EventHandler<SendCodeInputArgs> SendCodeInputEvent;
    public class SendCodeInputArgs : EventArgs { public int inputVal; }
    public void CodeInput(int inputVal)
    {
        SendCodeInputEvent?.Invoke(this, new SendCodeInputArgs { inputVal = inputVal });
    }

    public event EventHandler DisableCodeInputEvent;
    public void DisableCodeInput()
    {
        DisableCodeInputEvent?.Invoke(this, EventArgs.Empty);
    }

    public event EventHandler EnableCodeInputEvent;
    public void EnableCodeInput()
    {
        EnableCodeInputEvent?.Invoke(this, EventArgs.Empty);
    }

    public event EventHandler ButtonPressAudioEvent;
    public void PlayAudioButtonPress()
    {
        ButtonPressAudioEvent?.Invoke(this, EventArgs.Empty);
    }

}
