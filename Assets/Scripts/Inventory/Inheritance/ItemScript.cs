using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ItemScript : MonoBehaviour, IInteraction
{

    [Header("Variables")]
    [SerializeField] protected string itemName;
    [SerializeField] protected ItemSO.ItemList itemEnum;
    [SerializeField] protected int ammo;
    [SerializeField] protected string notif;
    [SerializeField] protected int maxAmmo;
    [SerializeField] protected bool oneTimeActivation = false;

    [SerializeField] protected TextAsset ifSameItemFound;

    protected Transform interactor;

    public event EventHandler OnInteractActive;
    public event EventHandler OnInteractDeactive;

    public virtual void OnInteract(Transform plr)
    {
        
    }

    public virtual void OnDetected(Transform plr)
    {

    }

    public virtual string UpdateNotif()
    {
        return notif;
    }
}
