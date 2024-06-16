using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class Handle : MonoBehaviour
{
    public Transform selectedItem;
    public ItemUses currentItemUses;

    public void ActivateMainUse(bool isClicked, Transform itemSource, Transform plr)
    {
        if (selectedItem != null && currentItemUses != null)
        {
            currentItemUses.MainUse(isClicked, itemSource, plr);
        }
        else
        {

        }
        
    }
}
