using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Pistol_ItemScript : ItemScript
{
    [Header("ScriptableObjects")]
    [SerializeField] ItemSO itemSO;



    private void OnEnable()
    {
        if(oneTimeActivation == false)
        {
            itemName = itemSO.name;
            maxAmmo = itemSO.amountAmmo;
            itemEnum = itemSO.currentItemEnum;
            oneTimeActivation = true;
            var newAmmo = Random.Range((float)maxAmmo / 2, (float)maxAmmo);
            ammo = (int)newAmmo;
            if (ammo > maxAmmo)
            {
                maxAmmo = ammo;
            }
            notif = itemSO.notif;
        }
        else
        {

        }
    }

    IEnumerator IsInteractDebounce;
    IEnumerator InteractDebounce()
    {
        var debTime = 0f;
        var debRate = 0.15f;
        while(debTime < debRate)
        {
            debTime += Time.deltaTime;
            yield return null;
        }
        IsInteractDebounce = null;
    }
    public override void OnInteract(Transform plr)
    {
        var inventory = plr.GetComponentInChildren<InventorySystem>();
        if (inventory != null && IsInteractDebounce == null)
        {
            IsInteractDebounce = InteractDebounce();
            StartCoroutine(IsInteractDebounce);
            var plrLgc = plr.TryGetComponent(out PlayerLogic lgc) ? lgc : null;
            CheckInventoryForItem(inventory, lgc);
        }
    }

    void CheckInventoryForItem(InventorySystem inventory, PlayerLogic Lgc)
    {
        if(inventory.GetInventory().transform.childCount > 0)
        {
            ItemUses item;
            for (int i = 0; i < inventory.GetInventory().transform.childCount; i++)
            {
                if(inventory.GetInventory().transform.GetChild(i).transform.TryGetComponent(out item))
                {
                    if ((int)item.GetItemEnum() == (int)itemEnum)
                    {
                        Lgc.PlayAudioEvent(new PlayerLogic.PlayAudioClipEventArgs { audioType = EntityAudioClipsSO.AudioTypes.Pickup });
                        var float_randomAmmo = Random.Range(3f, (float)ammo / 2f);
                        var int_intRandomAmmo = Mathf.RoundToInt(float_randomAmmo);
                        ammo -= int_intRandomAmmo;
                        Debug.Log(int_intRandomAmmo);
                        item.RefillAmmo(int_intRandomAmmo);
                        Destroy(transform.gameObject);
                        return;
                    }
                }  
            }
            //Instantiate(itemSO.prefab, inventory.GetInventory().transform);
            //Destroy(transform.gameObject);
            Debug.Log("Found nothing");
            Lgc.PlayAudioEvent(new PlayerLogic.PlayAudioClipEventArgs { audioType = EntityAudioClipsSO.AudioTypes.Pickup });
            var newItem = Instantiate(itemSO.inventoryPrefab, inventory.GetInventory().transform);
            inventory.AddItem(newItem);
            if (newItem.TryGetComponent(out ItemUses itemUses))
            {
                itemUses.SetAmmo(ammo);
            }
            Destroy(transform.gameObject);
            return;
        }
        else
        {
            //Instantiate(itemSO.prefab, inventory.GetInventory().transform);
            //(transform.gameObject);
            Lgc.PlayAudioEvent(new PlayerLogic.PlayAudioClipEventArgs { audioType = EntityAudioClipsSO.AudioTypes.Pickup });
            var newItem = Instantiate(itemSO.inventoryPrefab, inventory.GetInventory().transform);
            inventory.AddItem(newItem);
            if (newItem.TryGetComponent(out ItemUses itemUses))
            {
                itemUses.SetAmmo(ammo);
                Debug.Log(itemUses.GetAmmo());
            }
            Destroy(transform.gameObject);
            return;
        }
    }
}
