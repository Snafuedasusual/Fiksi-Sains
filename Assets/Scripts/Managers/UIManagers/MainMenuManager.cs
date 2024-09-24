using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    public static MainMenuManager instance;
    [SerializeField] GameObject mainPrefab;
    [SerializeField] GameObject menu;
    [SerializeField] GameObject controls;
    [SerializeField] GameObject credits;
    [SerializeField] GameObject canvas;
    [SerializeField] GameObject mainPrefabWorld;

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


    public void OpenMenu()
    {

    }

    public void ActivateMenu()
    {
        mainPrefabWorld.SetActive(true);
    }

    public void DeactivateMenu()
    {
        mainPrefabWorld.SetActive(false);
    }
}
