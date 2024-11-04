using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FactionBaseSO", menuName = "EnemyFactions/FactionBaseSO")]
public class FactionBaseSO : ScriptableObject
{
    public enum EnemyFactions { None,TerrorBirds, SkinEaters}
    public EnemyFactions enemyFactions;
}
