using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "KnifeUsesSO", menuName = "ItemUses/KnifeUsesSO")]
public class KnifeUsesSO : ItemUsesSO
{
    public float range;
    public float damage;
    public float coolDown;
    public float knckBckPwr;
    public RuntimeAnimatorController controller;
}
