using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IObjectiveSection
{
    public enum IsFinished
    {
        NotDone,
        IsDone
    }

    public enum IsLocked
    {
        Locked,
        Unlocked
    }

    public enum HasIndicator
    {
        Yes,
        No
    }



    public HasIndicator CanHaveIndicator();


    public void Unlocked()
    {

    }

    public void Lock()
    {

    }

    public void OnDone()
    {

    }

    public void ResetObj()
    {

    }

    public IsLocked LockOrUnlocked()
    {
        return IsLocked.Locked;
    }

    public string GetObjText()
    {
        return string.Empty;
    }

    public void ForceDone()
    {

    }
}
