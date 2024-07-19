using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "BaseSightStatesSO", menuName = "ScriptableObjects/EnemySightStates")]
public class BaseSightStatesSO : ScriptableObject
{
    public enum SightStates
    {
        HasNotSeen,
        HasSeen,
    }

    public SightStates sightStates;
}
