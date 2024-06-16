using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mouse_PlayerInput : MonoBehaviour
{
    [SerializeField] private Handle handleScr;
    [SerializeField] private Transform itemSource;

    private bool OnLClick()
    {
        bool isClicked = Input.GetMouseButton(0); 
        if (Input.GetMouseButton(0))
        {
            handleScr.ActivateMainUse(isClicked, itemSource, transform);
            return isClicked;
        }
        else
        {
            handleScr.ActivateMainUse(isClicked, itemSource, transform);
            return isClicked;
        }
    }

    public bool GetLClick()
    {
        return OnLClick();
    }


    private void Update()
    {
        OnLClick();
    }


}
