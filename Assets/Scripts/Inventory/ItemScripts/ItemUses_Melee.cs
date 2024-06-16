using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemUses_Melee : ItemUses
{
    [SerializeField] private string itemName;



    public override string GetName()
    {
        return itemName;
    }
}
