using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableUIManager : MonoBehaviour, ICloseAllMenus
{
    public static InteractableUIManager instance;
    [SerializeField] GameObject interactableUI;
    private GameObject currentUI;

    private void Awake()
    {
        if(instance != null && instance != this)
        {

        }
        else if(instance == null)
        {
            instance = this;
        }
    }

    public void ActivateInteractableUI(GameObject ui)
    {
        if(currentUI != null) return;
        currentUI = ui;
        //UIManager.instance.AddOpenedMenu(currentUI);
        currentUI.transform.SetParent(interactableUI.transform);
        currentUI.GetComponent<RectTransform>().SetParent(interactableUI.GetComponent<RectTransform>(), false);
        currentUI.GetComponent<RectTransform>().transform.position = interactableUI.GetComponent<RectTransform>().position;
        GameManagers.instance.SetStateToOnUI();
    }

    public void DeactivateInteractableUI()
    {
        Destroy(currentUI);
        currentUI = null;
        GameManagers.instance.SetStateToPlaying();
    }

    public void CloseMenu()
    {
        DeactivateInteractableUI();
    }

    Coroutine DebounceActiveText;
    IEnumerator StartDebounceActivetext()
    {
        var debTime = 0f;
        var debRate = 0.1f;
        while(debTime < debRate)
        {
            debTime += Time.deltaTime;
            yield return null;
        }
        DebounceActiveText = null;
    }
    public void ReadableText()
    {
        Debug.Log("ReadableTextStarted");
        if (currentUI == null) return;
        if (!currentUI.TryGetComponent(out IReadableUI readable))  return;
        if (DebounceActiveText != null) return;
        if (readable.GetTextVersion().activeSelf == false) { DebounceActiveText = StartCoroutine(StartDebounceActivetext()); readable.GetTextVersion().SetActive(true); return; }
        if (readable.GetTextVersion().activeSelf == true) { DebounceActiveText = StartCoroutine(StartDebounceActivetext()); ; readable.GetTextVersion().SetActive(false); return; }
    }
}
