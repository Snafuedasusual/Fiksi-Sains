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
        currentUI = ui;
        currentUI.transform.SetParent(interactableUI.transform);
        currentUI.GetComponent<RectTransform>().transform.position = interactableUI.GetComponent<RectTransform>().position;
        GameManagers.instance.SetStateToOnMenu();
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
}
