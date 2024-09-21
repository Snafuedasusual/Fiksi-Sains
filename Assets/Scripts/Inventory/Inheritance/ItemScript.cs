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
    [SerializeField] protected int maxAmmo;
    [SerializeField] protected bool oneTimeActivation = false;

    public event EventHandler OnInteractActive;
    public event EventHandler OnInteractDeactive;

    public virtual void OnInteract(Transform plr)
    {
        
    }

    public void OnDetected(Transform plr)
    {

    }
}
