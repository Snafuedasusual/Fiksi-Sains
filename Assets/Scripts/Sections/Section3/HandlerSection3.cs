using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class HandlerSection3 : BaseHandler, IInitializeScript
{
    [SerializeField] Transform plrStart;
    [SerializeField] Transform[] levers;
    [SerializeField] LayerMask lyrMask;
    [SerializeField] List<Transform> activeLevers;
    [SerializeField] int amountOfLevers;
    [SerializeField] int activatedLevers;
    [SerializeField] Door_Section doorSection;
    [SerializeField] GameObject skinEater;
    [SerializeField] Vector3 skinEaterPos;

    [Header("Script References")]
    [SerializeField] SectionEventComms sectionEventComms;

    private int[] ambianceClips;

    public void InitializeScript()
    {
        sectionEventComms.OnObjActivatedEvent += OnObjActivatedReceiver;
        sectionEventComms.OnObjDoneEvent += OnObjDoneEventReceiver;
        ambianceClips = new int[2];
        ambianceClips[0] = (int)AmbianceSO.AmbianceClips.AMBIANCE_SECT3_1;
        ambianceClips[1] = (int)AmbianceSO.AmbianceClips.AMBIANCE_SECT3_2;
    }


    public void DeInitializeScript()
    {
        sectionEventComms.OnObjActivatedEvent -= OnObjActivatedReceiver;
        sectionEventComms.OnObjDoneEvent -= OnObjDoneEventReceiver;
    }

    private void Start()
    {
        InitializeScript();
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

    private void SkinEaterSpawn()
    {
        skinEater.SetActive(true);
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
        IObjectiveSection newObj = objectives[currentObj].TryGetComponent(out IObjectiveSection newObjective) ? newObj = newObjective : null;
        newObj.Unlocked();
        ObjectiveTextManager.instance.UpdateText(newObj.GetObjText());
        skinEater.transform.position = skinEaterPos;
        skinEater.SetActive(false);
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
        IObjectiveSection newObj = objectives[currentObj].TryGetComponent(out IObjectiveSection newObjective) ? newObj = newObjective : null;
        newObj.Unlocked();
        ObjectiveTextManager.instance.UpdateText(newObj.GetObjText());
        skinEater.transform.position = skinEaterPos;
        skinEater.SetActive(false);
    }



    private void OnObjDoneEventReceiver(object sender, SectionEventComms.OnObjDoneEventArgs e)
    {
        UpdateObjective(e.obj);
    }

    private void OnObjActivatedReceiver(object sender, SectionEventComms.OnObjActivatedEventArgs e)
    {
        skinEater.SetActive(true);
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
            IsFinishLevelDebounce = FinishLevelDebounce();
            StartCoroutine(IsFinishLevelDebounce);
            GameManagers.instance.OnLevelChange(player.transform);
            ObjectiveTextManager.instance.EmptyText();
        }

    }

}
