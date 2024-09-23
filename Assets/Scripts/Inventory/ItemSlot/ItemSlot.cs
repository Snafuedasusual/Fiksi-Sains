using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemSlot : MonoBehaviour
{
    [Header("Script References")]
    [SerializeField] InventoryComms invComms;

    [Header("Variables")]
    [SerializeField] GameObject itemHeld;
    [SerializeField] GameObject itemUI;
    [SerializeField] string itemName;
    [SerializeField] string itemDesc;
    [SerializeField] int ammo;

    public void AddItem(GameObject newItem)
    {
        itemHeld = newItem;
        var newItemUses = itemHeld.GetComponent<ItemUses>();
        var newItemUI = Instantiate(newItemUses.GetItemUI(), transform);
        itemUI = newItemUI;
        ItemIcon itemUIScr = newItemUI.TryGetComponent(out ItemIcon item) ? itemUIScr = item : itemUIScr = null;
        newItemUI.GetComponent<RectTransform>().position = transform.GetComponent<RectTransform>().position;
        itemName = newItemUses.GetDisplayName();
        itemDesc = newItemUses.GetItemDesc();
        itemUIScr.ItemHolderAdder(this.gameObject);
        itemUIScr.SetInteractable();
    }

    public GameObject GetItemHeld()
    {
        return itemHeld;
    }

    public string GetItemName()
    {
        return itemName; 
    }

    public void DeleteItem()
    {
        itemHeld = null;
        if(itemUI != null) Destroy(itemUI.gameObject);
        itemUI = null;
        itemDesc = null;
        ammo = 0;
    }

    public void OnHoveredItem()
    {
        invComms.OnHoveredItemEvent(itemName, itemDesc, itemHeld);
    }

    public void OnExitItem()
    {
        invComms.ExitItemUI();
    }
}
