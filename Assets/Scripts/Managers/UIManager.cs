using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    [SerializeField] GameObject manyUIManagers;
    private List<GameObject> openedMenu;
    private int currentMenuIndex;

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


    public void AddOpenedMenu(GameObject menu)
    {
        openedMenu.Add(menu);
        if(openedMenu.Count > 0)
        {
            currentMenuIndex = openedMenu.Count - 1;
            for (int i = 0; i < openedMenu.Count; i++)
            {
                if (i == currentMenuIndex) break;
                openedMenu[i].gameObject.SetActive(false);
            }
        }
        else
        {
            currentMenuIndex = 0;
        }
    }

    public void RemoveOpenedMenu()
    {
        var toDestroy = openedMenu[currentMenuIndex].gameObject;
        openedMenu.RemoveAt(currentMenuIndex);
        Destroy(toDestroy);
        if(openedMenu.Count > 0)
        {
            currentMenuIndex--;
            openedMenu[currentMenuIndex].gameObject.SetActive(true);
        }
        else
        {
            currentMenuIndex = 0;
        }
    }


    public int GetCurrentOpenedMenu()
    {
        return openedMenu.Count;
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
