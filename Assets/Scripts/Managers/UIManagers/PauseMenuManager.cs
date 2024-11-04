using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenuManager : MonoBehaviour, ICloseAllMenus
{
    public static PauseMenuManager instance;
    [Header("Canvas")]
    [SerializeField] GameObject canvas;
    [Header("PauseMenu")]
    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameObject pauseMenuWorld;

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


    public void PauseMenuController()
    {
        if(GameManagers.instance.GetGameState() == GameManagers.GameState.OnMenu)
        {
            if(pauseMenuWorld != null)
            {
                pauseMenuWorld.GetComponent<IInitializeScript>().DeInitializeScript();
                Destroy(pauseMenuWorld);
            }
        }
        else
        {
            pauseMenuWorld = Instantiate(pauseMenu, canvas.transform);
            pauseMenu.transform.SetAsLastSibling();
            pauseMenu.SetActive(true);
            pauseMenuWorld.GetComponent<IInitializeScript>().InitializeScript();
        }
    }

    public void CloseMenu()
    {
        if(pauseMenuWorld != null)
        {
            pauseMenuWorld.GetComponent<IInitializeScript>().DeInitializeScript();
            Destroy(pauseMenuWorld);
        }
    }
}
