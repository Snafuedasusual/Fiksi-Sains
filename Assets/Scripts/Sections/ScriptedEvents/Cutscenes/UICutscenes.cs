using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICutscenes : MonoBehaviour, IScriptedEvents
{
    [SerializeField] GameObject uiCutscene;
    private IScriptedEvents.Triggered isTriggered = IScriptedEvents.Triggered.NotTriggered;
    private PlayerLogic playerLogic;

    public void ResetTrigger()
    {
        isTriggered = IScriptedEvents.Triggered.NotTriggered;
    }

    public void Trigger()
    {
        if (isTriggered == IScriptedEvents.Triggered.HasTriggered) return;
        if (CheckIfCutsceneFinished != null) return;
        playerLogic.NullifyState();
        CheckIfCutsceneFinished = StartCoroutine(StartCheckIfCutsceneFinished());
        var spawnedUI = Instantiate(uiCutscene);
        CutsceneUIManager.instance.ActivateCutsceneUI(spawnedUI);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent(out PlayerLogic lgc)) return;
        playerLogic = lgc;
        Trigger();
    }

    Coroutine CheckIfCutsceneFinished;
    IEnumerator StartCheckIfCutsceneFinished()
    {
        var timer = 0f;
        var maxTimer = 0.2f;
        while (timer < maxTimer)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        while (CutsceneUIManager.instance.GetIfCutsceneActive() == true)
        {
            if (CutsceneUIManager.instance.GetIfCutsceneActive() == false) { break; }
            yield return null;
        }
        CheckIfCutsceneFinished = null; 
        playerLogic.UnNullifyState(); 
        playerLogic = null; 
        isTriggered = IScriptedEvents.Triggered.HasTriggered;
    }
}
