using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour, IInteraction
{
    [SerializeField] GameObject readableUI;
    [SerializeField] string notif;
    public event EventHandler OnInteractActive;
    public event EventHandler OnInteractDeactive;

    bool isOpened = false;

    public void OnDetected(Transform plr)
    {
        throw new NotImplementedException();
    }

    Coroutine InteractDebounce;
    IEnumerator StartInteractDebounce()
    {
        var debTime = 0f;
        var debRate = 0.15f;
        while(debTime < debRate)
        {
            debTime += Time.deltaTime;
            yield return null;
        }
        InteractDebounce = null;
    }

    public void OnInteract(Transform plr)
    {
        if (InteractDebounce != null) return;
        InteractDebounce = StartCoroutine(StartInteractDebounce());
        PlayerLogic lgc = plr.TryGetComponent(out PlayerLogic logic) ? logic : null;
        if(isOpened == true)
        {
            if (lgc == null) return;
            lgc.plrState = PlayerLogic.PlayerStates.Idle;
            lgc.HideUI(false);
            isOpened = false;
            Debug.Log(isOpened);
            InteractableUIManager.instance.DeactivateInteractableUI();
        }
        else
        {
            if (lgc == null) return;
            lgc.plrState = PlayerLogic.PlayerStates.InteractingToggle;
            lgc.HideUI(true);
            isOpened = true;
            Debug.Log(isOpened);
            var readableUIWorld = Instantiate(readableUI, UIManager.instance.GetCanvas().transform);
            InteractableUIManager.instance.ActivateInteractableUI(readableUIWorld);
        }
    }

    public string UpdateNotif()
    {
        return notif;
    }
}
