using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemIcon : MonoBehaviour, IInitializeScript, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Scriptable Objects")]
    [SerializeField] FirearmSO itemSO;

    [Header("Variables")]
    [SerializeField] GameObject itemHolder;
    [SerializeField] string itemName;
    [SerializeField] string itemDesc;
    [SerializeField] int ammo;
    [SerializeField] bool isInteractable;

    public void InitializeScript()
    {
        itemName = itemSO.name;
        itemDesc = itemSO.itemDescription;
        ammo = itemSO.amountofAmmo;
    }

    public void DeInitializeScript()
    {
        throw new System.NotImplementedException();
    }

    private void Start()
    {
        InitializeScript();
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
