using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public class ItemScriptableObject : ScriptableObject
{
    public GameObject prefab;
    public string objName;
    public bool isFirearm;
    public float fireRange;
    public float fireDelay;
    public float fireDmg;
    public float reloadTime;

}
