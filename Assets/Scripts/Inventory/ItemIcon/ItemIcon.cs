using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemIcon : MonoBehaviour, IInitializeScript, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Scriptable Objects")]
    [SerializeField] ItemSO itemSO;

    [Header("Variables")]
    [SerializeField] GameObject itemHolder;
    [SerializeField] string itemName;
    [SerializeField] string itemDesc;
    [SerializeField] int ammo;
    [SerializeField] TextMeshProUGUI ammoUI;
    [SerializeField] bool isInteractable;

    public void InitializeScript()
    {
        itemName = itemSO.name;
        itemDesc = itemSO.itemDescription;
    }

    public void DeInitializeScript()
    {
        throw new System.NotImplementedException();
    }


    private void OnEnable()
    {
        InitializeScript();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isInteractable == true) { itemHolder.GetComponent<ItemSlot>().OnHoveredItem(); }
    }

    public void ItemHolderAdder(GameObject itemHold)
    {
        itemHolder = itemHold;
    }

    public void SetAmmoForUI(GameObject itemHolder)
    {
        Debug.Log("Played!");
        if(itemHolder.TryGetComponent(out ItemSlot slot))
        {
            if (slot.GetItemEnum() != ItemSO.ItemList.Pistol) { ammoUI.gameObject.SetActive(false); return; }
            ammoUI.gameObject.SetActive(true);
            ammo = slot.GetAmmoForUI();
            ammoUI.text = ammo.ToString();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (isInteractable == true) { itemHolder.GetComponent<ItemSlot>().OnExitItem(); }
    }

    public void SetInteractable()
    {
        isInteractable = true;
    }

    public void SetUnInteractable()
    {
        isInteractable = false;
    }
}
