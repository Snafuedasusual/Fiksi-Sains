using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    public static MainMenuManager instance;
    [SerializeField] GameObject mainPrefab;
    [SerializeField] GameObject menu;
    [SerializeField] GameObject controls;
    [SerializeField] GameObject credits;
    [SerializeField] GameObject canvas;
    [SerializeField] GameObject mainPrefabWorld;

    [SerializeField] string mainMenuScene;

    [SerializeField] RawImage blocker;

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
        if(DelayBeforeShow == null) { DelayBeforeShow = StartCoroutine(StartDelayBeforeShow()); }
        blocker.color = new Color(blocker.color.r, blocker.color.g, blocker.color.b, 1);
        mainPrefabWorld.SetActive(true);
        SceneManager.LoadSceneAsync(mainMenuScene, LoadSceneMode.Additive);
    }

    public void DeactivateMenu()
    {
        mainPrefabWorld.SetActive(false);
        SceneManager.UnloadSceneAsync(mainMenuScene);
    }

    Coroutine DelayBeforeShow;
    IEnumerator StartDelayBeforeShow()
    {
        var timer = 0f;
        var maxTimer = 3.5f;
        while(timer < maxTimer)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        for(float i = 1; i > 0f; i -= Time.deltaTime)
        {
            blocker.color = new Color(blocker.color.r, blocker.color.g, blocker.color.b, i);
            yield return null;
        }
        blocker.color = new Color(blocker.color.r, blocker.color.g, blocker.color.b, 0);
        DelayBeforeShow = null;
    }
}
