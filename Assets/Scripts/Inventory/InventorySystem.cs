using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventorySystem : MonoBehaviour, IInitializeScript
{
    [Header("Script References")]
    [SerializeField] PlayerToUI plrToUI;


    [SerializeField] private GameObject inventory;
    [SerializeField] GameObject hand;
    [SerializeField] private GameObject currentItem;
    private int amountOfItems;

    public void InitializeScript()
    {
        plrToUI.EquipItemEventSenderEvent += EquipItemEventSenderEventReceiver;
    }


    private void OnEnable()
    {
        InitializeScript();
    }

    private void OnDisable()
    {
        DeInitializeScript();
    }

    private void OnDestroy()
    {
        DeInitializeScript();
    }


    public void DeInitializeScript()
    {
        plrToUI.EquipItemEventSenderEvent -= EquipItemEventSenderEventReceiver;
    }

    public GameObject GetInventory()
    {
        return inventory;
    }

    public event EventHandler<OnAddItemArgs> OnAddItem;
    public class OnAddItemArgs : EventArgs { public GameObject item; }
    public void AddItem(GameObject newItem)
    {
        if (inventory.transform.childCount == amountOfItems) return;
        newItem.transform.parent = transform;
        OnAddItem?.Invoke(this, new OnAddItemArgs { item = newItem });
        newItem.SetActive(false);
    }

    private void DropItem(GameObject itemToDrop)
    {
        if(itemToDrop.transform.TryGetComponent(out ItemUses item))
        {
            item.DropItem();
        }
    }



    Coroutine StickItemToHandle;
    IEnumerator StartStickItemToHandle()
    {
        while(currentItem != null)
        {
            currentItem.transform.position = hand.transform.position;
            currentItem.transform.eulerAngles = new Vector3(hand.transform.eulerAngles.x, hand.transform.eulerAngles.y, hand.transform.eulerAngles.z);
            yield return null;
        }
        StickItemToHandle = null;
    }

    Coroutine EquipDebounce;
    IEnumerator EquipStart()
    {
        var debTime = 0f;
        var debRate = 0.15f;
        while(debTime < debRate)
        {
            debTime += Time.deltaTime;
            yield return null;
        }
        EquipDebounce = null;
    }
    private void EquipItemEventSenderEventReceiver(object sender, PlayerToUI.EquipItemEventSenderEventArgs e)
    {
        if (EquipDebounce != null) return;
        EquipItem(e.item);
        EquipDebounce = StartCoroutine(EquipStart());
    }
    public event EventHandler<EquipItemEventArgs> EquipItemEvent;
    public class EquipItemEventArgs : EventArgs { public GameObject senderObject; public GameObject item; }
    private void EquipItem(GameObject item)
    {
        EquipItemEvent?.Invoke(this, new EquipItemEventArgs {senderObject = transform.gameObject, item = item });
        if (currentItem != null)
        {
            currentItem.transform.position = transform.position;
            currentItem.SetActive(false);
            currentItem.transform.LookAt(transform.forward);
            currentItem = item;
            currentItem.SetActive(true);
            //currentItem.transform.LookAt(hand.transform.forward);
            Debug.Log("Replaced");
            return;
        }
        else if (currentItem == null)
        {
            currentItem = item;
            currentItem.SetActive(true);
            StickItemToHandle = StartCoroutine(StartStickItemToHandle());
            //currentItem.transform.LookAt(hand.transform.forward);
            Debug.Log("New Item");
            return;
        }
    }


}
