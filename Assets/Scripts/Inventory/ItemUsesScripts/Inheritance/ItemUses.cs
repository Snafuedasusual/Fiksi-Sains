using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ItemUses : MonoBehaviour
{


    [Header("Variables")]
    [SerializeField] protected string itemName;
    [SerializeField] protected ItemSO.ItemList itemEnum;
    [SerializeField] protected float fireCooldown;
    [SerializeField] protected float knockBackPwr;
    [SerializeField] protected float damage;
    [SerializeField] protected float range;
    [SerializeField] protected float ammo;
    [SerializeField] protected float maxAmmo;
    [SerializeField] protected string itemDesc;
    [SerializeField] protected GameObject itemUI;
    [SerializeField] protected bool oneTimeActivation = false;

    [Header("LayerMasks")]
    [SerializeField] protected LayerMask placesToDrop;
    [SerializeField] protected LayerMask characters;

    [SerializeField] protected RuntimeAnimatorController controller;
    public virtual RuntimeAnimatorController GetController() { return controller; }


    public virtual string GetDisplayName() { return itemName; }
    public virtual ItemSO.ItemList GetItemEnum() { return itemEnum; }
    public virtual string GetItemDesc() { return itemDesc; }
    public virtual GameObject GetItemUI() { return itemUI; }

    public virtual void MainUse(bool isClicked, Transform source, float heightPos)
    {

    }

    public virtual void RefillAmmo(int newAmmo)
    {

    }

    public virtual void DropItem()
    {

    }

    public virtual void SetAmmo(int newAmmo)
    {
        ammo = newAmmo;
    }
}