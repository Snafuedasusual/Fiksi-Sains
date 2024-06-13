using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemUses_Pistol : ItemUses
{
    [SerializeField]private string itemName;

    public override string GetName()
    {
        return itemName;
    }
}
