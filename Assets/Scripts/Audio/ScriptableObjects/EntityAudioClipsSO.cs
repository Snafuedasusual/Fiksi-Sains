using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EntityAudioClipsSO", menuName = "Sounds/EntityAudioClips")]
public class EntityAudioClipsSO : ScriptableObject
{
    public enum AudioTypes
    {
        Attack,
        Footsteps,
        Idle,
        Chase,
        Pickup,
        Hurt,
        Death,
    }
    public AudioClip[] attackAudios;
    public AudioClip[] footStepsClips;
    public AudioClip[] idleClips;
    public AudioClip[] chaseClips;
    public AudioClip[] pickupClips;
    public AudioClip[] hurtClips;
    public AudioClip[] deathClips;
}
