using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "EnemyBaseStats", menuName = "EnemyStats/EnemyBaseStats")]
public class BaseEnemyStatsSO : ScriptableObject
{
    [Header("Speeds")]
    public float defaultSpeed;
    public float defaultAccel;
    public float chaseSpeed;
    public float chaseAccel;

    [Header("Attack")]
    public float minDistToAttack;
    public float mainDmg;
    public float mainDmgKnckBck;
    public float mainDmgRange;
    public float secDmg;
    public float secDmgKnckBck;
    public float secDmgRange;
    public float thirdDmg;
    public float thirdDmgKnckBck;
    public float thirdDmgRange;
    public float fourthDmg;
    public float fourthDmgKnckBck;
    public float fourthDmgRange;

    [Header("Hearing")]
    public float hearingModifier;

    [Header("Proportions")]
    public Vector3 centerBody;
}
