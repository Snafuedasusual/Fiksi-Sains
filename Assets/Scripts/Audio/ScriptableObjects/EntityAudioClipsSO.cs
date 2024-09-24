using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EntityAudioClipsSO", menuName = "Sounds/EntityAudioClips")]
public class EntityAudioClipsSO : ScriptableObject
{
    public AudioClip[] attackAudios;
    public AudioClip[] footStepsClips;
    public AudioClip[] idleClips;
    public AudioClip[] chaseClips;
    public AudioClip[] hurtClips;
    public AudioClip[] deathClips;
}
