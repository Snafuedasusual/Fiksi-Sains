using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using static IObjectiveSection;

public class InteractObjectives : MonoBehaviour, IObjectiveSection, IInitializeScript
{
    [SerializeField] GameObject blockade; //if you want to block players to proceed before completing
    [SerializeField] GameObject[] objToInteract;
    [SerializeField] int amountOfInteractions;
    private int interactionCount = 0;

    [SerializeField] SectionEventComms sectionEventComms;

    [SerializeField] IObjectiveSection.IsFinished currentStatus;
    [SerializeField] IObjectiveSection.IsLocked currentLockStatus;

    [SerializeField] string objText;


    public void InitializeScript()
    {
        for (int i = 0; i < objToInteract.Length; i++)
        {
            if (!objToInteract[i].TryGetComponent(out IInteraction interact)) { }
            else interact.OnInteractActive += OnInteractActiveReciever;
        }
    }

    public void DeInitializeScript()
    {
        for (int i = 0; i < objToInteract.Length; i++)
        {
            if (!objToInteract[i].TryGetComponent(out IInteraction interact)) { }
            else interact.OnInteractActive -= OnInteractActiveReciever;
        }
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


    Coroutine ReceiveDebounce;
    IEnumerator StartReceiveDebounce()
    {
        var timer = 0f;
        var maxTimer = 0.1f;
        while(timer < maxTimer)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        ReceiveDebounce = null;
    }
    private void OnInteractActiveReciever(object sender, System.EventArgs e)
    {
        if (ReceiveDebounce != null) return;
        ReceiveDebounce = StartCoroutine(StartReceiveDebounce());
        interactionCount++;
        Debug.Log(interactionCount);
        if (interactionCount >= amountOfInteractions) OnDone();
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
        if (blockade == null) return;
        blockade.SetActive(false);
    }

    public void ResetObj()
    {
        currentStatus = IsFinished.NotDone;
        currentLockStatus = IsLocked.Locked;
        for(int i = 0; i < objToInteract.Length; i++)
        {
            objToInteract[i].SetActive(true);
        }
        interactionCount = 0;
        if (blockade == null) return;
        blockade.SetActive(true);
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

}
