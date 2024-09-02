using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenuHandler : MonoBehaviour,IInitializeScript
{
    [Header("Components")]
    [SerializeField] Button resume;
    [SerializeField] Button restart;
    [SerializeField] Button quit;

    public void InitializeScript()
    {
        resume.onClick.AddListener(delegate { GameManagers.instance.EscInputReceiver(); });
        restart.onClick.AddListener(delegate { GameManagers.instance.OnPlayerDeathRestart(); });
        restart.onClick.AddListener(delegate { this.DestroyPauseMenu(); });
        quit.onClick.AddListener(delegate { GameManagers.instance.QuitGame(); });
    }

    public void DeInitializeScript() 
    {
        resume.onClick.RemoveListener(delegate { GameManagers.instance.EscInputReceiver(); });
        restart.onClick.RemoveListener(delegate {GameManagers.instance.OnPlayerDeathRestart(); });
        restart.onClick.RemoveListener(delegate { this.DestroyPauseMenu(); });
        quit.onClick.RemoveListener(delegate { GameManagers.instance.QuitGame(); });
    }

    private void DestroyPauseMenu()
    {
        Destroy(this.gameObject);
    }

    private void Start()
    {
       InitializeScript();
    }

    private void OnEnable()
    {
        InitializeScript();
    }

    private void OnDisable()
    {
        DeInitializeScript();
    }

    private void OnDestroy()
    {
        DeInitializeScript();
    }
}
