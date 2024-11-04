using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MusicSO", menuName = "Music/MusicSO")]
public class MusicSO : ScriptableObject
{
    public enum ChaseMusic
    {
        CHASE_SECT_2,
    }

    public enum NormalMusic
    {
        INTRO_RADIO,
        ENDING_AND_TITLE
    }

    public AudioClip[] chaseMusic;
    public AudioClip[] normalMusic;
}
