using System;
using System.Collections;
using System.Collections.Generic;
using Unity.PlasticSCM.Editor.WebApi;
using UnityEngine;

public class InventorySystem : MonoBehaviour, IInitializeScript
{
    [Header("Script References")]
    [SerializeField] PlayerToUI plrToUI;


    [SerializeField] private GameObject inventory;

    public void InitializeScript()
    {
        plrToUI.EquipItemEventSenderEvent += EquipItemEventSenderEventReceiver;
    }


    private void Start()
    {
        InitializeScript();
    }


    public void DeInitializeScript()
    {
        throw new NotImplementedException();
    }

    public GameObject GetInventory()
    {
        return inventory;
    }

    public event EventHandler<OnAddItemArgs> OnAddItem;
    public class OnAddItemArgs : EventArgs { public GameObject item; }
    public void AddItem(GameObject newItem)
    {
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

    private void EquipItemEventSenderEventReceiver(object sender, PlayerToUI.EquipItemEventSenderEventArgs e)
    {
        EquipItem(e.item);
    }
    public event EventHandler<EquipItemEventArgs> EquipItemEvent;
    public class EquipItemEventArgs : EventArgs { public GameObject senderObject; public GameObject item; }
    private void EquipItem(GameObject item)
    {
        EquipItemEvent?.Invoke(this, new EquipItemEventArgs {senderObject = transform.gameObject, item = item });
    }


}
