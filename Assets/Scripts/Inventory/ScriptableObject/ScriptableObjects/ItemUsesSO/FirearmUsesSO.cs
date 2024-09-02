using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FirearmUsesSO", menuName = "Firearm/FirearmUsesSO")]
public class FirearmUsesSO : ItemUsesSO
{
    public float range;
    public int ammo;
    public float damage;
    public float coolDown;
    public float knckBckPwr;
}
