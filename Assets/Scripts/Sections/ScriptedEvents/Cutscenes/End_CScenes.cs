using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class End_CScenes : MonoBehaviour, IScriptedEvents
{
    [SerializeField] SectionEventComms sectionEventComms;
    [SerializeField] private IScriptedEvents.Triggered isTriggered;

    [Header("Components")]
    [SerializeField] GameObject human;
    [SerializeField] GameObject tntcl;
    [SerializeField] Transform humanPos;
    [SerializeField] Transform tntclPos;
    [SerializeField] Vector3 humanAngle;
    [SerializeField] Vector3 tntclAngle;

    private void Awake()
    {
        sectionEventComms.ScriptedEventNoArgs += ScriptedEventNoArgsReceiver; 
    }

    private void ScriptedEventNoArgsReceiver(object sender, System.EventArgs e)
    {
        Trigger();
    }

    public void ResetTrigger()
    {
        isTriggered = IScriptedEvents.Triggered.NotTriggered;
    }


    Coroutine IsCountdownToDisable;
    IEnumerator StartCountdownToDisable()
    {
        var count = 0f;
        var maxCount = 7.5f;
        while(count < maxCount)
        {
            count += Time.deltaTime;
            yield return null;
        }
        IsCountdownToDisable = null;
        human.SetActive(false);
        tntcl.SetActive(false);
    }
    public void Trigger()
    {
        Debug.Log("Played");
        if (isTriggered == IScriptedEvents.Triggered.HasTriggered) return;
        if (IsCountdownToDisable != null) return;
        IsCountdownToDisable = StartCoroutine(StartCountdownToDisable());
        SetOriginalPos();
        human.SetActive(true);
        tntcl.SetActive(true);
    }

    void SetOriginalPos()
    {
        human.transform.position = humanPos.position;
        human.transform.eulerAngles = humanAngle;
        tntcl.transform.position = tntclPos.position;
        tntcl.transform.eulerAngles = tntclAngle;
    }
}
