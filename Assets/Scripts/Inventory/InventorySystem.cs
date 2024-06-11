using System.Collections;
using System.Collections.Generic;
using Unity.PlasticSCM.Editor.WebApi;
using UnityEngine;

public class InventorySystem : MonoBehaviour
{
    [SerializeField] private GameObject inventory;
    [SerializeField] private PlayerInput plrInpt;
    [SerializeField] private Transform Handle;

    private int Selectedinventory;
    private bool canSelect;

    private void InventoryChecker()
    {
        if (inventory.transform.childCount == 0)
        {
            canSelect = false;
        }
        else
        {
            canSelect = true;
        }
        
    }

    private void ScrollInventory()
    {
        if (plrInpt.GetSwitchInventoryCode() > 0f && canSelect)
        {

            if (Selectedinventory >= inventory.transform.childCount - 1)
            {
                Selectedinventory = 0;
                TestEquip();
            }
            else
            {
                Selectedinventory++;
                TestEquip();
            }
        }
        if (plrInpt.GetSwitchInventoryCode() < 0f && canSelect)
        {
            if (Selectedinventory <= 0)
            {
                Selectedinventory = inventory.transform.childCount - 1;
                TestEquip();
            }
            else
            {
                Selectedinventory--;
                TestEquip();
            }
        }
        //Debug.Log(Selectedinventory);
    }

    private Transform TestEquip()
    {
        Transform currentItem = null;
        Transform currentItemVisual = null;
        for (int i = 0; i < inventory.transform.childCount; i++)
        {
            if (i == Selectedinventory)
            {
                currentItem = inventory.transform.GetChild(i);
                currentItem.transform.position = Handle.position;
                for(int j = 0; j < currentItem.transform.childCount; j++)
                {
                    currentItemVisual = currentItem.transform.GetChild(j);
                    currentItemVisual.gameObject.SetActive(true);
                }
            }
            else
            {
                inventory.transform.GetChild(i).transform.position = inventory.transform.position;
                //inventory.transform.GetChild(i).transform.GetChild(0).gameObject.SetActive(false);
                for(int j = 0; j < inventory.transform.GetChild(i).transform.childCount; j++)
                {
                    inventory.transform.GetChild(i).transform.GetChild(j).gameObject.SetActive(false);
                }
            }

        }
        return currentItem;
    }

    public Transform GetCurrentItem()
    {
        return TestEquip();
    }

    private void Update()
    {
        InventoryChecker();
        ScrollInventory();
    }

    private void Start()
    {
        TestEquip();
    }




}
