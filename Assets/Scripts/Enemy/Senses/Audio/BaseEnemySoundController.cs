using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEnemySoundController : MonoBehaviour, IMakeSound
{
    [SerializeField] BaseEnemyStatsSO statsSO;
    [Header("Variables")]
    [SerializeField] float hearingModifier;


    private void Start()
    {
        hearingModifier = statsSO.hearingModifier; 
    }

    private void OnEnable()
    {
        hearingModifier = statsSO.hearingModifier;
    }

    public void SoundProducer(float soundAdder)
    {
        
    }

    public event EventHandler<SoundToLogicArgs> SoundToLogic;
    public class SoundToLogicArgs : EventArgs 
    { 
        public float alertLevel; 
        public Vector3 location; 
    }
    public void SoundReceiver(Vector3 soundSrc, float soundSize)
    {
        var lowAlertLevel = 1f;
        var highAlertLevel = 2f;
        var distance = Vector3.Distance(soundSrc, transform.position);
        if(distance < (soundSize * hearingModifier) && distance > (soundSize * hearingModifier) * 0.75f)
        {
            SoundToLogic?.Invoke(this, new SoundToLogicArgs { alertLevel = lowAlertLevel, location = soundSrc });
        }
        else if(distance < (soundSize * hearingModifier) * 0.75f)
        {
            SoundToLogic?.Invoke(this, new SoundToLogicArgs { alertLevel = highAlertLevel, location = soundSrc });
        }
        else
        {

        }
    }
}
