using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    [SerializeField] private string[] sectionNames;
    [SerializeField] private string mainSceneName;
    [SerializeField] private BaseHandler currentHandler;
    private List<AsyncOperation> sceneToLoad = new List<AsyncOperation>();

    public List<GameObject> savedItems = new List<GameObject>();

    public enum GameState
    {
        MainMenu,
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
    public GameObject GetPlayer() { return plr; }


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
        PlayerUIManager.instance.DeActivateUI();
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
        if(currentState == GameState.Dead || currentState == GameState.MainMenu)
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
                var plrLgc = plr.TryGetComponent(out PlayerLogic lgc) ? lgc : null;
                if (plrLgc == null) return;
                plrLgc.HideUI(true);
            }
            else if (currentState == GameState.OnMenu && IsEscDebounce == null)
            {
                IsEscDebounce = EscDebounce();
                StartCoroutine(IsEscDebounce);
                UIManager.instance.CloseAllMenus();
                var plrLgc = plr.TryGetComponent(out PlayerLogic lgc) ? lgc : null;
                if (plrLgc == null) return;
                plrLgc.HideUI(false);
            }
            else if (currentState == GameState.OnUI && IsEscDebounce == null)
            {
                IsEscDebounce = EscDebounce();
                StartCoroutine(IsEscDebounce);
                UIManager.instance.CloseAllMenus();
                var plrLgc = plr.TryGetComponent(out PlayerLogic lgc) ? lgc : null;
                if (plrLgc == null) return;
                plrLgc.HideUI(false);
            }
        }
    }

    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;
        var plrLgc = plr.TryGetComponent(out PlayerLogic lgc) ? lgc : null;
    }

    public void UnpauseGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
        var plrLgc = plr.TryGetComponent(out PlayerLogic lgc) ? lgc : null;
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

    public void SetStateToMainMenu()
    {
        currentState = GameState.MainMenu;
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
        UnpauseGame();
        plr = GameObject.FindGameObjectWithTag("Player");
        plr.GetComponent<PlayerLogic>().NullifyState();
        Screen.SetResolution(640, 360, true);
        SetStateToMainMenu();
        MainMenuManager.instance.ActivateMenu();
        if (currentState == GameState.FreeCam) return;
        if (handlers.Length <= 0) return;
        //handlers[currentLevel - 1].player = plr;
        //StartCoroutine(IsStartingLevel(plr.transform));
    }

    Coroutine PlayDebounce;
    IEnumerator StartPlayDebounce()
    {
        var timer = 0f;
        var maxTimer = 0.1f;
        while (timer < maxTimer)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        PlayDebounce = null;
    }
    public void PlayGame()
    {
        if (PlayDebounce != null) return;
        PlayDebounce = StartCoroutine(StartPlayDebounce());
        LoadScenes();
    }

    Coroutine RestartDebounce;
    IEnumerator StartRestartDebounce()
    {
        var timer = 0f;
        var maxTimer = 0.1f;
        while(timer < maxTimer)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        RestartDebounce = null;
    }
    public void RestartGame()
    {
        if (RestartDebounce != null) return;
        RestartDebounce = StartCoroutine(StartRestartDebounce());
        LoadSceneRestart();
    }

    private void LoadScenes()
    {
        AmbianceManager.instance.RefreshAudio();
        if (currentHandler != null) { currentHandler.player = null; }
        currentHandler = null;
        LoadingScreenManager.instance.ActivateLoading();
        for(int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene loadedScene = SceneManager.GetSceneAt(i);
            if (loadedScene.name == mainSceneName) { }
            if (loadedScene.name != sectionNames[currentLevel - 1] && loadedScene.name != mainSceneName)
            {
                SceneManager.UnloadSceneAsync(loadedScene);
            }
        }
        sceneToLoad.Add(SceneManager.LoadSceneAsync(sectionNames[currentLevel - 1], LoadSceneMode.Additive));
        StartCoroutine(StartLoadingScene(false));
    }

    private void LoadSceneRestart()
    {
        if (currentHandler != null) { currentHandler.player = null; }
        currentHandler = null;
        LoadingScreenManager.instance.ActivateLoading();
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene loadedScene = SceneManager.GetSceneAt(i);
            if (loadedScene.name == mainSceneName) { }
            if (loadedScene.name != sectionNames[currentLevel - 1] && loadedScene.name != mainSceneName) SceneManager.UnloadSceneAsync(loadedScene);
            if (loadedScene.name == sectionNames[currentLevel - 1] && loadedScene.name != mainSceneName) SceneManager.UnloadSceneAsync(loadedScene);
        }
        sceneToLoad.Add(SceneManager.LoadSceneAsync(sectionNames[currentLevel - 1], LoadSceneMode.Additive));
        StartCoroutine(StartLoadingScene(true));
    }

    IEnumerator StartLoadingScene(bool restart)
    {
        plr.transform.position = loadingSpot.position;
        plr.transform.GetComponent<PlayerLogic>().NullifyState();
        CheckifSceneLoaded(restart);
        Debug.Log("StartLoadingSceneFunc");
        for (int i = 0; i < sceneToLoad.Count; i++)
        {
            while (!sceneToLoad[i].isDone)
            {
                yield return null;
            }
        }
        sceneToLoad.Clear();
    }

    private void CheckifSceneLoaded(bool restart)
    {
        if (CheckSceneLoaded != null) return;
        StartCoroutine(StartCheckSceneLoaded(restart));
    }
    Coroutine CheckSceneLoaded;
    IEnumerator StartCheckSceneLoaded(bool restart)
    {
        var timer = 0f;
        var maxTimer = 3.5f;
        while (true)
        {
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                Scene loadedScene = SceneManager.GetSceneAt(i);
                if (loadedScene.name == mainSceneName) { }
                if (loadedScene.name == sectionNames[currentLevel - 1] && loadedScene.name != mainSceneName)
                {
                    while(currentHandler == null)
                    {
                        currentHandler = FindObjectOfType<BaseHandler>();
                        yield return null;
                    }
                    if (restart == true) RestartSection();
                    else StartSection();
                    CheckSceneLoaded = null;
                    yield break;
                }
            }
            yield return null;
        }
    }

    public void StartSection()
    {
        if (currentHandler == null) { Debug.Log("Null!"); return; }
        LoadingScreenManager.instance.DeactivateLoading();
        UIManager.instance.CloseAllMenus();
        ChaseMusicManager.instance.StopMusic();
        GenericMusicManager.instance.StopMusic();
        currentHandler.player = plr;
        currentHandler.StartLevel();
        plr.transform.GetComponent<PlayerLogic>().UnNullifyState();
        SetStateToPlaying();
        PlayerLogic plrLgc = plr.TryGetComponent(out PlayerLogic logic) ? logic : null;
        logic.UnHidePlayer();
        logic.UnNullifyState();
    }

    public void NextLevel()
    {
        if (currentLevel < sectionNames.Length)
        {
            currentLevel++;
            SaveItems();
            LoadScenes();
        }
        else
        {
            ClearItems();
            CreditsManager.instance.ActivateCredits();
            currentLevel = 1;
        }
    }

    public void RestartSection()
    {
        if (currentHandler == null) { Debug.Log("Null!"); return; }
        LoadItems();
        LoadingScreenManager.instance.DeactivateLoading();
        UIManager.instance.CloseAllMenus();
        ChaseMusicManager.instance.StopMusic();
        GenericMusicManager.instance.StopMusic();
        currentHandler.player = plr;
        currentHandler.Restart();
        SetStateToPlaying();
        UnpauseGame();
        PlayerLogic plrLgc = plr.TryGetComponent(out PlayerLogic logic) ? logic : null;
        logic.ResetPlayer();
        logic.UnHidePlayer();
        logic.UnNullifyState();
        plr.SetActive(true);
        UIManager.instance.DeactivateGameOver();
        PlayerUIManager.instance.ActivateUI();
    }




    private void SaveItems()
    {
        if (plr == null) return;
        InventorySystem inventory = plr.GetComponentInChildren<InventorySystem>();
        if (inventory == null) return;
        savedItems.Clear();
        if (inventory.GetInventory().transform.childCount < 1) return;
        for (int i = 0; i < inventory.GetInventory().transform.childCount; i++)
        {
            var duplicateItem = Instantiate(inventory.GetInventory().transform.GetChild(i).gameObject, transform);
            duplicateItem.SetActive(false);
            savedItems.Add(duplicateItem);
        }
    }



    private void LoadItems()
    {
        if (plr == null) return;
        InventorySystem inventory = plr.GetComponentInChildren<InventorySystem>();
        if (inventory == null) return;
        if (inventory.GetInventory().transform.childCount < 1) return;
        InventoryMenuManager.instance.ClearItems();
        if (savedItems.Count < 1) return;
        for (int i = 0; i < savedItems.Count; i++)
        {
            var item = Instantiate(savedItems[i].gameObject);
            inventory.AddItem(item);
        }
    }



    private void ClearItems()
    {
        savedItems.Clear();
        if (plr == null) return;
        InventorySystem inventory = plr.GetComponentInChildren<InventorySystem>();
        if (inventory == null) return;
        for (int i = 0; i < inventory.GetInventory().transform.childCount; i++)
        {
            Destroy(inventory.GetInventory().transform.GetChild(i).gameObject);
        }
    }

}
