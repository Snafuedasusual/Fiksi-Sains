using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandlerSection2 : BaseHandler, IInitializeScript
{
    [Header("StartLogic")]
    [SerializeField] Transform plrStart;
    [SerializeField] Vector3[] deadScientistLocation;
    [SerializeField] Transform deadScientist;
    [SerializeField] Door_Section Section;

    [Header("Script References")]
    [SerializeField] SectionEventComms sectionEventComms;

    [SerializeField] GameObject Enemies;

    private int[] ambianceClips;

    public void CanFinish()
    {
        //Section.canFinish = true;
    }

    private void Start()
    {
        deadScientist.localPosition = deadScientistLocation[Random.Range(0, deadScientistLocation.Length - 1)];
    }

    private void OnEnable()
    {
        deadScientist.localPosition = deadScientistLocation[Random.Range(0, deadScientistLocation.Length - 1)];
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

    public void InitializeScript()
    {
        sectionEventComms.OnObjDoneEvent += OnObjDoneEventReceiver;
        ambianceClips = new int[2];
        ambianceClips[0] = (int)AmbianceSO.AmbianceClips.AMBIANCE_SECT2_1;
        ambianceClips[1] = (int)AmbianceSO.AmbianceClips.AMBIANCE_SECT2_2;
    }

    public void DeInitializeScript()
    {
        sectionEventComms.OnObjDoneEvent -= OnObjDoneEventReceiver;
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
        if(scriptedEvents.Length > 0)
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
        ResetTheEnemies();
        AmbianceManager.instance.RequestPlay(ambianceClips);
    }

    public override void Restart()
    {
        currentObj = 0;
        deadScientist.localPosition = deadScientistLocation[Random.Range(0, deadScientistLocation.Length - 1)];
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
        ResetTheEnemies();
    }

    private void ResetTheEnemies()
    {
        if(Enemies.transform.childCount > 0) for (int i = 0; i < Enemies.transform.childCount; i++)
            {
                if(Enemies.transform.GetChild(i).transform.TryGetComponent(out BaseEnemyLogic logic))
                {
                    logic.ResetEnemy();
                }
            }
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
            if (objectives[i] == gameObject && i < objectives.Length - 1 && i > currentObj)
            {
                currentObj = i;
                IObjectiveSection pastCurrentObj = objectives[currentObj].TryGetComponent(out IObjectiveSection objSelect) ? objSelect : null;
                FinishObjectivesBetween(currentObj, i);
                pastCurrentObj.ForceDone();
                UnlockNextObjective();
            }
            if (objectives[i] == gameObject && i == objectives.Length - 1 && i > currentObj)
            {
                currentObj = i;
                IObjectiveSection pastCurrentObj = objectives[currentObj].TryGetComponent(out IObjectiveSection objSelect) ? objSelect : null;
                FinishObjectivesBetween(currentObj, i);
                pastCurrentObj.ForceDone();
                FinishLevel();
            }
            else
            {
                FinishLevel();
            }
        }
    }

    protected override void UnlockNextObjective()
    {
        if (objectives[currentObj + 1].TryGetComponent(out IObjectiveSection objective))
        {
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
            AmbianceManager.instance.RefreshAudio();
            IsFinishLevelDebounce = FinishLevelDebounce();
            StartCoroutine(IsFinishLevelDebounce);
            GameManagers.instance.OnLevelChange(player.transform);
            ObjectiveTextManager.instance.EmptyText();
        }

    }
}
