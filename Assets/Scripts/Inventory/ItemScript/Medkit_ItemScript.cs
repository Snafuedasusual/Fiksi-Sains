using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Medkit_ItemScript : ItemScript
{
    [SerializeField] ItemSO medkitSO;


    private void OnEnable()
    {
        if (oneTimeActivation == false)
        {
            itemName = medkitSO.name;
            notif = medkitSO.notif;
            itemEnum = medkitSO.currentItemEnum;
            oneTimeActivation = true;
            if (ammo > maxAmmo)
            {
                maxAmmo = ammo;
            }
            notif = medkitSO.notif;
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
        while (debTime < debRate)
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
        if (inventory.GetInventory().transform.childCount > 0)
        {
            ItemUses item;
            for (int i = 0; i < inventory.GetInventory().transform.childCount; i++)
            {
                if (inventory.GetInventory().transform.GetChild(i).transform.TryGetComponent(out item))
                {
                    if ((int)item.GetItemEnum() == (int)itemEnum)
                    {
                        return;
                    }
                }
            }
            //Instantiate(itemSO.prefab, inventory.GetInventory().transform);
            //Destroy(transform.gameObject);
            Lgc.PlayAudioEvent(new PlayerLogic.PlayAudioClipEventArgs { audioType = EntityAudioClipsSO.AudioTypes.Pickup });
            Debug.Log("Found nothing");
            var newItem = Instantiate(medkitSO.inventoryPrefab, inventory.GetInventory().transform);
            inventory.AddItem(newItem);
            Destroy(transform.gameObject);
        }
        else
        {
            //Instantiate(itemSO.prefab, inventory.GetInventory().transform);
            //(transform.gameObject);
            Lgc.PlayAudioEvent(new PlayerLogic.PlayAudioClipEventArgs { audioType = EntityAudioClipsSO.AudioTypes.Pickup });
            var newItem = Instantiate(medkitSO.inventoryPrefab, inventory.GetInventory().transform);
            inventory.AddItem(newItem);
            Destroy(transform.gameObject);
            return;
        }
    }
}
