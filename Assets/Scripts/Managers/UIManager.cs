using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    [SerializeField] GameObject manyUIManagers;

    private void Awake()
    {
        if(instance != null && instance != this)
        {

        }
        else
        {
            instance = this;
        }
    }

    public GameObject GetManyUIManagersObject()
    {
        return manyUIManagers;
    }

    public void CloseAllMenus()
    {
        var menus = manyUIManagers.GetComponentsInChildren<ICloseAllMenus>();
        for (int i = 0; i < menus.Length; i++)
        {
            menus[i].CloseMenu();
        }
    }

    public void ActivateGameOver()
    {
        GameOverManager.instance.ActivateGameOver();
    }

    public void DeactivateGameOver()
    {
        GameOverManager.instance.DeactivateGameOver();
    }
}
