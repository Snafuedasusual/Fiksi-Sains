using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BaseSightSO", menuName = "ScriptableObjects/EnemySight")]
public class BaseSightSO : ScriptableObject
{
    public float maxRayDist;
    public float maxVision;
    public float minDotProduct;
}
