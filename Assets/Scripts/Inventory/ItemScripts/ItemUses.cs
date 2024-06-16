using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemUses : MonoBehaviour
{
    private string thisName;
    public virtual void MainUse(bool isClicked, Transform source, Transform plr)
    {

    }

    public virtual string GetName()
    {
        return thisName;
    }
}
