using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseHandler : MonoBehaviour
{
    public GameObject player;
    [Header("Objectives")]
    [SerializeField] protected GameObject[] objectives;
    [Header("ScriptedEvents")]
    [SerializeField] protected GameObject[] scriptedEvents;
    [SerializeField] protected int currentObj;

    protected int[] ambianceClips;
    protected int chaseClips;
    public virtual int GetChaseMusicClip()
    {
        return chaseClips;
    }

    public virtual void Restart()
    {
        
    }

    public virtual void StartLevel()
    {

    }

    protected virtual void UpdateObjective(GameObject gameObject)
    {

    }

    protected virtual void UnlockNextObjective()
    {

    }

    protected virtual void FinishObjectivesBetween(int currentObj, int skippedObj)
    {

    }

    protected virtual void FinishLevel()
    {

    }
}
