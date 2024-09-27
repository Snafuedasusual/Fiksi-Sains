using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static AmbianceSO;

public class FinalHandler : BaseHandler, IInitializeScript
{
    [Header("Script References")]
    [SerializeField] SectionEventComms sectEventComms;

    [Header("Scriptable Events")]
    [SerializeField] Transform plrStart;
    [SerializeField] Door_Section end;


    public void InitializeScript()
    {
        sectEventComms.OnObjDoneEvent += OnObjDoneEventReceiver;
        ambianceClips = new int[2];
        chaseClips = 1000;
        ambianceClips[0] = (int)AmbianceSO.AmbianceClips.AMBIANCE_SECT4_1;
        ambianceClips[1] = (int)AmbianceSO.AmbianceClips.AMBIANCE_SECT4_2;
    }

    public void DeInitializeScript()
    {
        sectEventComms.OnObjDoneEvent -= OnObjDoneEventReceiver;
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

    public override void StartLevel()
    {
        currentObj = 0;
        player.transform.position = plrStart.position;
        for (int i = 0; i < objectives.Length; i++)
        {
            if (objectives[i].TryGetComponent(out IObjectiveSection objective))
            {
                objective.ResetObj();
            }
        }
        if (scriptedEvents.Length > 0)
        {
            for (int i = 0; i < scriptedEvents.Length; i++)
            {
                if (scriptedEvents[i].TryGetComponent(out IScriptedEvents events))
                {
                    events.ResetTrigger();
                }
            }
        }
        IObjectiveSection newObj = objectives[currentObj].TryGetComponent(out IObjectiveSection newObjective) ? newObj = newObjective : null;
        newObj.Unlocked();
        ObjectiveTextManager.instance.UpdateText(newObj.GetObjText());
        AmbianceManager.instance.RequestPlay(ambianceClips);
    }

    public override void Restart()
    {
        currentObj = 0;
        player.transform.position = plrStart.position;
        for (int i = 0; i < objectives.Length; i++)
        {
            if (objectives[i].TryGetComponent(out IObjectiveSection objective))
            {
                objective.ResetObj();
            }
        }
        if (scriptedEvents.Length > 0)
        {
            for (int i = 0; i < scriptedEvents.Length; i++)
            {
                if (scriptedEvents[i].TryGetComponent(out IScriptedEvents events))
                {
                    events.ResetTrigger();
                }
            }
        }
        IObjectiveSection newObj = objectives[currentObj].TryGetComponent(out IObjectiveSection newObjective) ? newObj = newObjective : null;
        newObj.Unlocked();
        ObjectiveTextManager.instance.UpdateText(newObj.GetObjText());
    }

    private void OnObjDoneEventReceiver(object sender, SectionEventComms.OnObjDoneEventArgs e)
    {
        UpdateObjective(e.obj);
    }

    protected override void UpdateObjective(GameObject gameObject)
    {
        for (int i = 0; i < objectives.Length; i++)
        {
            if (objectives[i] == gameObject && i < objectives.Length - 1 && i == currentObj)
            {
                currentObj = i;
                UnlockNextObjective();
                break;
            }
            else if (objectives[i] == gameObject && i < objectives.Length - 1 && i > currentObj)
            {
                currentObj = i;
                IObjectiveSection pastCurrentObj = objectives[currentObj].TryGetComponent(out IObjectiveSection objSelect) ? objSelect : null;
                FinishObjectivesBetween(currentObj, i);
                pastCurrentObj.ForceDone();
                UnlockNextObjective();
                return;
            }
            else if (objectives[i] == gameObject && i == objectives.Length - 1 && i > currentObj)
            {
                currentObj = i;
                IObjectiveSection pastCurrentObj = objectives[currentObj].TryGetComponent(out IObjectiveSection objSelect) ? objSelect : null;
                FinishObjectivesBetween(currentObj, i);
                pastCurrentObj.ForceDone();
                FinishLevel();
                return;
            }
            else if (objectives[i] == gameObject && i == objectives.Length - 1 && i == currentObj)
            {
                FinishLevel();
            }
        }
    }

    protected override void UnlockNextObjective()
    {
        if (objectives[currentObj + 1].TryGetComponent(out IObjectiveSection objective))
        {
            currentObj += 1;
            objective.Unlocked();
            ObjectiveTextManager.instance.UpdateText(objective.GetObjText());
        }
    }

    protected override void FinishObjectivesBetween(int currentObj, int skippedObj)
    {
        IObjectiveSection objectiveSection;
        for (int i = skippedObj; currentObj < i; i--)
        {
            if (i < currentObj || i == currentObj)
            {

            }
            if (objectives[i] == objectives[currentObj])
            {

            }
            if (objectives[i] != objectives[currentObj] && objectives[i] != objectives[skippedObj])
            {
                if (objectives[i].TryGetComponent(out objectiveSection))
                {
                    objectiveSection.ForceDone();
                }
            }
        }
    }

    IEnumerator IsFinishLevelDebounce;
    IEnumerator FinishLevelDebounce()
    {
        var debTime = 0f;
        var debRate = 0.1f;
        while (debTime < debRate)
        {
            debTime += Time.deltaTime;
            yield return null;
        }
        IsFinishLevelDebounce = null;
    }
    protected override void FinishLevel()
    {
        if (IsFinishLevelDebounce == null)
        {
            IsFinishLevelDebounce = FinishLevelDebounce();
            StartCoroutine(IsFinishLevelDebounce);
            GameManagers.instance.NextLevel();
            ObjectiveTextManager.instance.EmptyText();
        }

    }


    public void ActivateEnd()
    {
        //end.canFinish = true;
    }
}
