using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemSO", menuName = "Item/ItemSO")]
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
    public GameObject inventoryPrefab;
    public GameObject uiIcon;
    public string itemName;
    public string itemDescription;
    public int amountAmmo;
    public string notif;
}
