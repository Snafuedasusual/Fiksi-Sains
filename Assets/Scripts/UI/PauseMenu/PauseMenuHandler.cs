using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenuHandler : MonoBehaviour,IInitializeScript
{
    [Header("Components")]
    [SerializeField] Button resume;
    [SerializeField] Button controlsButton;
    [SerializeField] Button restart;
    [SerializeField] Button quit;
    [SerializeField] TextMeshProUGUI obj;

    [SerializeField] Button back;

    [SerializeField] GameObject pause;
    [SerializeField] GameObject controls;

    public void InitializeScript()
    {
        resume.onClick.AddListener(delegate { GameManagers.instance.EscInputReceiver(); });
        controlsButton.onClick.AddListener(delegate { DisablePauseMenu(); });
        controlsButton.onClick.AddListener(delegate { EnableControls(); });
        restart.onClick.AddListener(delegate { GameManagers.instance.RestartGame(); });
        restart.onClick.AddListener(delegate { this.DestroyPauseMenu(); });
        quit.onClick.AddListener(delegate { GameManagers.instance.QuitGame(); });
        obj.text = ObjectiveTextManager.instance.GetObjectiveText();

        back.onClick.AddListener(delegate { DisableControls(); });
        back.onClick.AddListener(delegate { EnablePauseMenu(); });
    }

    public void DeInitializeScript() 
    {
        resume.onClick.RemoveListener(delegate { GameManagers.instance.EscInputReceiver(); });
        controlsButton.onClick.RemoveListener(delegate { DisablePauseMenu(); });
        controlsButton.onClick.RemoveListener(delegate { EnableControls(); });
        restart.onClick.RemoveListener(delegate {GameManagers.instance.RestartGame(); });
        restart.onClick.RemoveListener(delegate { this.DestroyPauseMenu(); });
        quit.onClick.RemoveListener(delegate { GameManagers.instance.QuitGame(); });
        obj.text = string.Empty;

        back.onClick.RemoveListener(delegate { DisableControls(); });
        back.onClick.RemoveListener(delegate { EnablePauseMenu(); });
    }

    private void DestroyPauseMenu()
    {
        Destroy(this.gameObject);
    }



    private void DisablePauseMenu()
    {
        pause.SetActive(false);
    }
    private void EnablePauseMenu()
    {
        pause.SetActive(true);
    }


    private void EnableControls()
    {
        controls.SetActive(true);
    }
    private void DisableControls()
    {
        controls.SetActive(false);
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
