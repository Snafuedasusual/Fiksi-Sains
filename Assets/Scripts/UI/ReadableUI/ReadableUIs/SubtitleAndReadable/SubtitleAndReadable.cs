using Ink.Parsed;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubtitleAndReadable : MonoBehaviour, IInteraction
{
    [SerializeField] GameObject readableUI;
    [SerializeField] string notif;
    [SerializeField] TextAsset dialogue;
    public event EventHandler OnInteractActive;
    public event EventHandler OnInteractDeactive;

    [SerializeField] bool isOpened = false;
    bool hasActivated = false;

    PlayerInput plrInp;
    PlayerLogic lgc;

    public void OnDetected(Transform plr)
    {
        throw new NotImplementedException();
    }

    Coroutine InteractDebounce;
    IEnumerator StartInteractDebounce()
    {
        var debTime = 0f;
        var debRate = 0.15f;
        while (debTime < debRate)
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
        lgc = plr.TryGetComponent(out PlayerLogic logic) ? logic : null;
        plrInp = plr.TryGetComponent(out PlayerInput inp) ? inp : null;
        if (isOpened == true)
        {
            if (lgc == null) return;
            isOpened = false;
            lgc.plrState = PlayerLogic.PlayerStates.Idle;
            lgc.HideUI(false);
            plrInp = null;
            lgc = null;
            InteractableUIManager.instance.DeactivateInteractableUI();
            if (dialogue == null) { if(hasActivated == false) hasActivated = true; return; } 
            if (hasActivated == true) return;
            SubtitleManager.instance.ActivateSubtitle(dialogue);
            if(hasActivated == false) hasActivated = true;
        }
        else
        {
            if (lgc == null) return;
            isOpened = true;
            lgc.plrState = PlayerLogic.PlayerStates.InteractingToggle;
            lgc.HideUI(true);
            CheckInput = StartCoroutine(StartCheckInput());
            var readableUIWorld = Instantiate(readableUI, UIManager.instance.GetCanvas().transform);
            InteractableUIManager.instance.ActivateInteractableUI(readableUIWorld);
        }
    }

    Coroutine CheckInputDebounce;
    IEnumerator StartCheckInputDebounce()
    {
        var debTime = 0f;
        var debRate = 0.15f;
        while(debTime < debRate)
        {
            debTime += Time.deltaTime;
            yield return null;
        }
        CheckInputDebounce = null;
    }

    Coroutine CheckInput;
    IEnumerator StartCheckInput()
    {
        while(isOpened == true)
        {
            if(plrInp.GetEscInput() == true)
            {
                if (CheckInputDebounce != null) { }
                else
                {
                    if (lgc == null) { CheckInput = null; yield break; };
                    CheckInputDebounce = StartCoroutine(StartCheckInputDebounce());
                    lgc.plrState = PlayerLogic.PlayerStates.Idle;
                    lgc.HideUI(false);
                    isOpened = false;
                    plrInp = null;
                    lgc = null;
                    InteractableUIManager.instance.DeactivateInteractableUI();
                    if (dialogue == null) { if (hasActivated == false) hasActivated = true; CheckInput = null; yield break; }
                    if (hasActivated == true) { CheckInput = null; yield break; };
                    SubtitleManager.instance.ActivateSubtitle(dialogue);
                    if (hasActivated == false) hasActivated = true;
                }
            }
            if(plrInp.GetSpaceInput() == true)
            {
                if (CheckInputDebounce != null) { }
                else { CheckInputDebounce = StartCoroutine(StartCheckInputDebounce());  InteractableUIManager.instance.ReadableText(); }
            }
            yield return null;
        }
        CheckInput = null;
    }

    public string UpdateNotif()
    {
        return notif;
    }
}
