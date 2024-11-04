using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryComms : MonoBehaviour
{
    public event EventHandler<OnHoveredItemUIArgs> OnHoveredItemUI;
    public class OnHoveredItemUIArgs : EventArgs { public string itemName; public string itemDesc; public GameObject item; }
    public void OnHoveredItemEvent(string name, string desc, GameObject item)
    {
        OnHoveredItemUI?.Invoke(this, new OnHoveredItemUIArgs { itemName = name, itemDesc = desc, item = item });
    }

    public event EventHandler OnExitItemUI;
    public void ExitItemUI()
    {
        OnExitItemUI?.Invoke(this, EventArgs.Empty);
    }
}
