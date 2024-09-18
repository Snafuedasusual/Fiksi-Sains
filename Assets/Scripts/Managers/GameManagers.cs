using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagers : MonoBehaviour
{
    public static GameManagers instance;
    [Header("Levels")]
    [SerializeField] Transform loadingSpot;
    [SerializeField] GameObject[] sections;
    [SerializeField] BaseHandler[] handlers;
    [SerializeField] int currentLevel;
    [SerializeField] GameObject plr;
    [SerializeField] private bool isPaused = false;

    public enum GameState
    {
        Playing,
        Dead,
        OnMenu,
        OnUI,
        FreeCam,
    }

    [Header("Game States")]
    [SerializeField] private GameState currentState;
    public GameState GetGameState() { return currentState; }
    public GameObject GetCurrentSection() { return sections[currentLevel - 1]; }
    public BaseHandler GetCurrentHandler() { return handlers[currentLevel - 1]; }
    public bool GetIsPaused()
    {
        return isPaused;
    }

    public void OnPlayerDeathRestart()
    {
        handlers[currentLevel - 1].Restart();
        if(currentState != GameState.Playing)
        {
            currentState = GameState.Playing;
        }
        SetStateToPlaying();
        UnpauseGame();
        PlayerLogic plrLgc = plr.TryGetComponent(out PlayerLogic logic) ? logic : null;
        logic.ResetPlayer();
        plr.SetActive(true);
        UIManager.instance.DeactivateGameOver();
    }



    IEnumerator IsGameOverActivated;
    IEnumerator GameOverActivate()
    {
        var debTime = 0f;
        var debRate = 0.2f;
        while(debTime < debRate)
        {
            debTime += Time.deltaTime;
            yield return null;
        }
        UIManager.instance.ActivateGameOver();
        IsGameOverActivated = null;
    }
    private void GameOver()
    {
        if(IsGameOverActivated == null)
        {
            IsGameOverActivated = GameOverActivate();
            StartCoroutine(IsGameOverActivated);
        }
    }
    public void OnPlayerDeath()
    {
        UIManager.instance.CloseAllMenus();
        currentState = GameState.Dead;
        PauseGame();
        SetStateToDead();
        UIManager.instance.ActivateGameOver();
    }



    public void OnPlayerLoadLevel()
    {
        handlers[currentLevel - 1].StartLevel();
        if (currentState != GameState.Playing)
        {
            currentState = GameState.Playing;
        }
        SetStateToPlaying();
        UnpauseGame();
    }

    public void StartLevel()
    {
        handlers[currentLevel - 1].StartLevel();
        if (currentState != GameState.Playing)
        {
            currentState = GameState.Playing;
        }
        SetStateToPlaying();
        UnpauseGame();
    }


    IEnumerator IsEscDebounce;
    IEnumerator EscDebounce()
    {
        var debTime = 0f;
        var debRate = 0.15f;
        if(isPaused == false && currentState != GameState.OnUI)
        {
            while (debTime < debRate)
            {
                debTime += Time.deltaTime;
                yield return null;
            }
            PauseGame();
            SetStateToOnMenu();
            IsEscDebounce = null;
        }
        else if(isPaused == true && currentState != GameState.OnUI)
        {
            UnpauseGame();
            SetStateToPlaying();
            while (debTime < debRate)
            {
                debTime += Time.deltaTime;
                yield return null;
            }
            IsEscDebounce = null;
        }
        else if(isPaused == false && currentState == GameState.OnUI)
        {
            UnpauseGame();
            SetStateToPlaying();
            while (debTime < debRate)
            {
                debTime += Time.deltaTime;
                yield return null;
            }
            IsEscDebounce = null;
        }
        else if (isPaused == true && currentState == GameState.OnUI)
        {
            UnpauseGame();
            SetStateToPlaying();
            while (debTime < debRate)
            {
                debTime += Time.deltaTime;
                yield return null;
            }
            IsEscDebounce = null;
        }

    }
    public void EscInputReceiver()
    {
        if(currentState == GameState.Dead)
        {

        }
        else
        {
            //Spawns Pause Menu
            if (currentState == GameState.Playing && IsEscDebounce == null)
            {
                IsEscDebounce = EscDebounce();
                StartCoroutine(IsEscDebounce);
                PauseMenuManager.instance.PauseMenuController();
            }
            else if(currentState == GameState.OnMenu && IsEscDebounce == null)
            {
                IsEscDebounce = EscDebounce();
                StartCoroutine(IsEscDebounce);
                UIManager.instance.CloseAllMenus();
            }
            else if(currentState == GameState.OnUI && IsEscDebounce == null)
            {
                IsEscDebounce = EscDebounce();
                StartCoroutine(IsEscDebounce);
                UIManager.instance.CloseAllMenus();
            }
        }
    }

    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;
    }

    public void UnpauseGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
    }

    public void SetStateToOnMenu()
    {
        currentState = GameManagers.GameState.OnMenu;
    }

    public void SetStateToPlaying()
    {
        currentState = GameState.Playing;
    }

    public void SetStateToDead()
    {
        currentState = GameState.Dead;
    }

    public void SetStateToOnUI()
    {
        currentState = GameState.OnUI;
    }


    public void OnLevelChange(Transform plr)
    {
        if(currentLevel < sections.Length)
        {
            currentLevel++;
           if(IsLoading == null)
           {
                sections[currentLevel - 2].gameObject.SetActive(false);
                IsLoading = LoadingLevel(plr);
                StartCoroutine(IsLoading);
           }
        }
        else
        {
            Debug.Log("End Of The Line!");
        }
    }

    IEnumerator IsLoading;
    IEnumerator LoadingLevel(Transform plr)
    {
        plr.transform.position = loadingSpot.position;
        plr.transform.GetComponent<PlayerLogic>().plrState = PlayerLogic.PlayerStates.Hiding;
        var loadTime = 0f;
        var loadRate = 0.07f;

        for(int i = 0; i < sections[currentLevel-1].transform.childCount; i++)
        {
            while (loadTime < loadRate)
            {
                loadTime += Time.deltaTime;
                yield return 0;
            }
            loadTime = 0f;
            sections[currentLevel - 1].transform.GetChild(i).transform.gameObject.SetActive(true);
            if (sections[currentLevel - 1].transform.GetChild(i).childCount > 0)
            {
                sections[currentLevel - 1].transform.GetChild(i).transform.gameObject.SetActive(true);
                for(int j = 0; j < sections[currentLevel - 1].transform.GetChild(i).childCount; j++)
                {
                    while (loadTime < loadRate)
                    {
                        loadTime += Time.deltaTime;
                        yield return 0;
                    }
                    loadTime = 0f;
                    sections[currentLevel - 1].transform.GetChild(i).GetChild(j).gameObject.SetActive(true);
                }
            }
        }
        plr.transform.GetComponent<PlayerLogic>().plrState = PlayerLogic.PlayerStates.Idle;
        handlers[currentLevel - 1].player = plr.gameObject;
        IsLoading = null;
        OnPlayerLoadLevel();
    }

    IEnumerator IsStarting;
    IEnumerator IsStartingLevel(Transform plr)
    {
        plr.transform.position = loadingSpot.position;
        plr.transform.GetComponent<PlayerLogic>().plrState = PlayerLogic.PlayerStates.Hiding;
        var loadTime = 0f;
        var loadRate = 0.07f;

        for (int i = 0; i < sections[currentLevel - 1].transform.childCount; i++)
        {
            while (loadTime < loadRate)
            {
                loadTime += Time.deltaTime;
                yield return 0;
            }
            loadTime = 0f;
            sections[currentLevel - 1].transform.GetChild(i).transform.gameObject.SetActive(true);
            if (sections[currentLevel - 1].transform.GetChild(i).childCount > 0)
            {
                sections[currentLevel - 1].transform.GetChild(i).transform.gameObject.SetActive(true);
                for (int j = 0; j < sections[currentLevel - 1].transform.GetChild(i).childCount; j++)
                {
                    while (loadTime < loadRate)
                    {
                        loadTime += Time.deltaTime;
                        yield return 0;
                    }
                    loadTime = 0f;
                    sections[currentLevel - 1].transform.GetChild(i).GetChild(j).gameObject.SetActive(true);
                }
            }
        }
        plr.transform.GetComponent<PlayerLogic>().plrState = PlayerLogic.PlayerStates.Idle;
        handlers[currentLevel - 1].player = plr.gameObject;
        IsLoading = null;
        StartLevel();
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    private void Start()
    {
        instance = this;
        isPaused = false;
        plr = GameObject.FindGameObjectWithTag("Player");
        Screen.SetResolution(640, 360, true);
        handlers[currentLevel - 1].player = plr;
        //StartCoroutine(IsStartingLevel(plr.transform));
    }

}
