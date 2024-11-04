using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemUsesSO: ScriptableObject
{
    public GameObject itemWorldPrefab;
    public AudioClip[] attackClips;
    public float soundRange;
}
