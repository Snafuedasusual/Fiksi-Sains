using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemSlot : MonoBehaviour
{
    [Header("Script References")]
    [SerializeField] InventoryComms invComms;

    [Header("Variables")]
    [SerializeField] GameObject itemHeld;
    [SerializeField] ItemSO.ItemList itemEnum;
    [SerializeField] GameObject itemUI;
    [SerializeField] string itemName;
    [SerializeField] string itemDesc;
    [SerializeField] int ammo;
    public ItemSO.ItemList GetItemEnum() { return itemEnum; }
    public int GetAmmoForUI() { return ammo; }

    public void AddItemUnInteractable(GameObject newItem)
    {
        itemHeld = newItem;
        var newItemUses = itemHeld.GetComponent<ItemUses>();
        var newItemUI = Instantiate(newItemUses.GetItemUI(), transform);
        itemEnum = newItemUses.GetItemEnum();
        itemUI = newItemUI;
        ItemIcon itemUIScr = newItemUI.TryGetComponent(out ItemIcon item) ? item : itemUIScr = null;
        newItemUI.GetComponent<RectTransform>().position = transform.GetComponent<RectTransform>().position;
        itemName = newItemUses.GetDisplayName();
        itemDesc = newItemUses.GetItemDesc();
        itemUIScr.ItemHolderAdder(this.gameObject);
        itemUIScr.SetUnInteractable();
        UpdateItem();
    }

    public void AddItemInteractable(GameObject newItem)
    {
        itemHeld = newItem;
        var newItemUses = itemHeld.GetComponent<ItemUses>();
        var newItemUI = Instantiate(newItemUses.GetItemUI(), transform);
        itemEnum = newItemUses.GetItemEnum();
        itemUI = newItemUI;
        ItemIcon itemUIScr = newItemUI.TryGetComponent(out ItemIcon item) ? item : itemUIScr = null;
        newItemUI.GetComponent<RectTransform>().position = transform.GetComponent<RectTransform>().position;
        itemName = newItemUses.GetDisplayName();
        itemDesc = newItemUses.GetItemDesc();
        itemUIScr.ItemHolderAdder(this.gameObject);
        itemUIScr.SetInteractable();
        UpdateItem();
    }

    public void UpdateItem()
    {
        if (itemHeld == null) return;
        ItemIcon icon = itemUI.TryGetComponent(out ItemIcon itemIcon) ? itemIcon : null;
        var newItemUses = itemHeld.GetComponent<ItemUses>();
        ammo = newItemUses.GetAmmo();
        if (icon == null) return;
        if (newItemUses.GetItemEnum() != ItemSO.ItemList.Pistol) return;
        icon.SetAmmoForUI(gameObject);
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
        itemEnum = 0;
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
