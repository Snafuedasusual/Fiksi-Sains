using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static IObjectiveSection;

public class LeverObjective : MonoBehaviour, IObjectiveSection
{
    [Header("Script References")]
    [SerializeField] SectionEventComms sectionEventComms;

    [Header("Variables")]
    [SerializeField] Transform[] levers;
    [SerializeField] LayerMask lyrMask;
    [SerializeField] List<Transform> activeLevers;
    [SerializeField] int amountOfLevers;
    [SerializeField] int activatedLevers;

    [SerializeField] string objText;
    [SerializeField] IObjectiveSection.IsFinished currentStatus;
    [SerializeField] IObjectiveSection.IsLocked currentLockStatus;
    [SerializeField] IObjectiveSection.HasIndicator canIndicate;

    float minDistance = 18;

    private void SetLevers()
    {
        DeactivateAllLevers();
        for (int i = 0; i < amountOfLevers; i++)
        {
            var selectedLever = levers[Random.Range(0, levers.Length - 1)].transform;
            if (activeLevers.Count > 0)
            {
                for (int j = 0; j < activeLevers.Count; j++)
                {
                    var dist = Vector3.Distance(activeLevers[j].position, selectedLever.position);
                    if (dist < minDistance || activeLevers[j].transform == selectedLever.transform)
                    {
                        break;
                    }
                    else
                    {
                        if (j == activeLevers.Count - 1)
                        {
                            selectedLever.gameObject.SetActive(true);
                            activeLevers.Add(selectedLever);
                        }
                    }
                }
            }
            else if (activeLevers.Count == 0)
            {
                selectedLever.gameObject.SetActive(true);
                activeLevers.Add(selectedLever);
            }
        }
        if (activeLevers.Count > amountOfLevers)
        {
            activeLevers.RemoveAt(activeLevers.Count - 1);
        }
        LeverChecker();
    }

    private void LeverChecker()
    {
        if (activeLevers.Count < amountOfLevers)
        {
            for (int i = 0; i < levers.Length; i++)
            {
                for (int j = 0; j < activeLevers.Count; j++)
                {
                    float distance = Vector3.Distance(activeLevers[j].position, levers[i].position);
                    if (distance < minDistance || activeLevers[j].transform == levers[i].transform)
                    {
                        break;
                    }
                    else
                    {
                        if (j == activeLevers.Count - 1)
                        {
                            activeLevers.Add(levers[i].transform);
                            levers[i].gameObject.SetActive(true);
                        }
                    }
                }
                if (activeLevers.Count == amountOfLevers)
                {
                    break;
                }
                else
                {

                }
            }
        }
    }

    private void DeactivateAllLevers()
    {
        activeLevers.Clear();
        for(int i = 0; i < levers.Length; i++)
        {
            levers[i].gameObject.SetActive(false);
        }
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
        SetLevers();
        activatedLevers = 0;
        currentStatus = IsFinished.NotDone;
        currentLockStatus = IsLocked.Locked;
    }

    public IsLocked LockOrUnlocked()
    {
        return IsLocked.Locked;
    }

    public string GetObjText()
    {
        objText = $"Find and activate the {activeLevers.Count} levers. {activatedLevers} out of {activeLevers.Count} remaining";
        return objText;
    }

    public void ForceDone()
    {

    }

    public void InteractedLever()
    {
        activatedLevers++;
        if( activatedLevers == amountOfLevers && currentLockStatus == IsLocked.Unlocked && currentStatus == IsFinished.NotDone)
        {
            OnDone();
        }
        if(activatedLevers == 1 && currentLockStatus == IsLocked.Unlocked && currentStatus == IsFinished.NotDone)
        {
            sectionEventComms.OnObjectiveActivated(gameObject);
        }
        if(activatedLevers < amountOfLevers)
        {
            ObjectiveTextManager.instance.UpdateText(GetObjText());
        }
    }

    public HasIndicator CanHaveIndicator()
    {
        return canIndicate;
    }
}
