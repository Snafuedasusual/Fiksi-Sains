using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static IObjectiveSection;

public class WallPasswordObjective : MonoBehaviour, IInteraction, IObjectiveSection
{
    [SerializeField] GameObject wallPasswordUI;


    [SerializeField] SectionEventComms sectionEventComms;
    [SerializeField] string objText;
    [SerializeField] string notif;
    [SerializeField] IObjectiveSection.IsFinished currentStatus;
    [SerializeField] IObjectiveSection.IsLocked currentLockStatus;
    [SerializeField] IObjectiveSection.HasIndicator canIndicate;

    public event EventHandler OnInteractActive;
    public event EventHandler OnInteractDeactive;

    IEnumerator IsInteractionDebounce;
    IEnumerator InteractionDebounce()
    {
        var debTime = 0f;
        var debRate = 0.15f;
        while(debTime < debRate)
        {
            debTime += Time.deltaTime;
            yield return null;
        }
        IsInteractionDebounce = null;
    }

    private bool isInteracting = false;


    public void OnInteract(Transform plr)
    {
        if(currentLockStatus == IsLocked.Unlocked && currentStatus == IsFinished.NotDone)
        {
            if (isInteracting == false && IsInteractionDebounce == null)
            {
                isInteracting = true;
                IsInteractionDebounce = InteractionDebounce();
                StartCoroutine(IsInteractionDebounce);
                var spawnedUI = Instantiate(wallPasswordUI, UIManager.instance.GetCanvas().transform);
                InteractableUIManager.instance.ActivateInteractableUI(spawnedUI);

                IUIObjectives iUI = spawnedUI.TryGetComponent(out iUI) ? iUI : null;
                iUI.AddListener(transform.gameObject);

                RectTransform rtUI = spawnedUI.TryGetComponent(out rtUI) ? rtUI : null;
                rtUI.localScale = Vector3.one;
            }
            else if (isInteracting == true && IsInteractionDebounce == null)
            {
                isInteracting = false;
                IsInteractionDebounce = InteractionDebounce();
                StartCoroutine(IsInteractionDebounce);
                InteractableUIManager.instance.DeactivateInteractableUI();
            }
        }
    }

    public void OnDetected(Transform plr)
    {
        throw new System.NotImplementedException();
    }

    public void Unlocked()
    {
        currentLockStatus = IsLocked.Unlocked;
    }

    public void Lock()
    {
        currentLockStatus = IsLocked.Locked;
    }

    public void OnDone()
    {
        sectionEventComms.OnObjectiveDone(gameObject);
    }

    public void ResetObj()
    {
        currentStatus = IsFinished.NotDone;
        currentLockStatus = IsLocked.Locked;
    }

    public IsLocked LockOrUnlocked()
    {
        return IsLocked.Locked;
    }

    public string GetObjText()
    {
        return objText;
    }

    public void ForceDone()
    {

    }

    public string UpdateNotif()
    {
        return notif;
    }

    public HasIndicator CanHaveIndicator()
    {
        return canIndicate;
    }
}
