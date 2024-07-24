using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "EnemyBaseStats", menuName = "EnemyStats/EnemyBaseStats")]
public class BaseEnemyStatesSO : ScriptableObject
{
    [Header("Speeds")]
    public float defaultSpeed;
    public float defaultAccel;
    public float chaseSpeed;
    public float chaseAccel;

    [Header("Min Range to Attack")]
    public float minDistToAttack;
}
