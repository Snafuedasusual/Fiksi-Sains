using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AmbianceClips", menuName = "Sounds/AmbianceClips")]
public class AmbianceSO : ScriptableObject
{
    public AudioClip[] audioClips;

    public enum AmbianceClips
    {
        AMBIANCE_SECT1_1,
        AMBIANCE_SECT1_2,
        AMBIANCE_SECT2_1,
        AMBIANCE_SECT2_2,
        AMBIANCE_SECT3_1,
        AMBIANCE_SECT3_2,
        AMBIANCE_SECT4_1,
        AMBIANCE_SECT4_2,
    }
}
