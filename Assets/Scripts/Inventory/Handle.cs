using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class Handle : MonoBehaviour, IInitializeScript
{
    public Transform selectedItem;
    public ItemUses currentItemUses;

    [Header("Script References")]
    [SerializeField] InventorySystem invSys;

    public void DeInitializeScript()
    {
        throw new System.NotImplementedException();
    }

    public void InitializeScript()
    {
        //invSys.EquipItemEvent += EquipItemEventReceiver;
    }

    public void Start()
    {
        InitializeScript();
    }

    public void ActivateMainUse(bool isClicked, Transform itemSource, Transform plr)
    {
        if (selectedItem != null && currentItemUses != null)
        {
            currentItemUses.MainUse(isClicked, itemSource, 1.1f);
        }
        else
        {

        }
        
    }

    private void EquipItemEventReceiver(object sender, InventorySystem.EquipItemEventArgs e)
    {
        EquipItemToHandler(e.senderObject, e.item);
    }
    private void EquipItemToHandler(GameObject sender, GameObject item)
    {
        if(selectedItem == null && currentItemUses == null)
        {
            currentItemUses = item.TryGetComponent(out ItemUses itemUses) ? currentItemUses = itemUses : null;
            selectedItem = item.transform;
            selectedItem.position = transform.position;
            selectedItem.eulerAngles = transform.eulerAngles;
            selectedItem.gameObject.SetActive(true);
        }
        else if(selectedItem != null && currentItemUses != null)
        {
            GameObject inventory = sender.TryGetComponent(out InventorySystem system) ? inventory = system.GetInventory() : null;
            currentItemUses = null;
            selectedItem.transform.position = inventory.transform.position;
            selectedItem.gameObject.SetActive(false);
            selectedItem = null;

            currentItemUses = item.TryGetComponent(out ItemUses itemUses) ? currentItemUses = itemUses : null;
            selectedItem = item.transform;
            selectedItem.position = transform.position;
            selectedItem.eulerAngles = transform.eulerAngles;
            selectedItem.gameObject.SetActive(true);
        }
    }

}
