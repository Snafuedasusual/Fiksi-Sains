using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ItemSO : ScriptableObject
{
    public enum ItemList
    {
        None,
        Pistol,
        Knife,
        Medkit
    }
    public ItemList currentItemEnum;
    public GameObject prefab;
    public GameObject uiIcon;
    public string itemName;
    public string itemDescription;
    
}
