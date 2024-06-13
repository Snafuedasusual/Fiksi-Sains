using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemUses : MonoBehaviour
{
    private string thisName;
    public virtual void OnLClick()
    {

    }

    public virtual void OnEPress()
    {

    }

    public virtual string GetName()
    {
        return thisName;
    }
}
