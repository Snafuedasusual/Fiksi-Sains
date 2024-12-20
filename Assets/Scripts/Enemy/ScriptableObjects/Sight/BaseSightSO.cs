using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BaseSightSO", menuName = "EnemySight/EnemyBaseSight")]
public class BaseSightSO : ScriptableObject
{
    public float eyeHeight;
    public float maxRayDist;
    public float maxVision;
    public float minDotProduct;
    public float minVisibilityBar;
}
