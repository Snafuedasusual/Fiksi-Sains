using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "MedkitUsesSO", menuName = "ItemUses/MedkitUsesSO")]
public class MedkitUsesSO : ItemUsesSO
{
    public int ammo;
    public float healRate;
    public RuntimeAnimatorController controller;
}
