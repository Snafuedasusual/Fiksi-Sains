using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SectionEventComms : MonoBehaviour
{
    public event EventHandler<OnObjDoneEventArgs> OnObjDoneEvent;
    public class OnObjDoneEventArgs : EventArgs {  public GameObject obj; }
    public void OnObjectiveDone(GameObject obj)
    {
        OnObjDoneEvent?.Invoke(this, new OnObjDoneEventArgs { obj = obj });
    }

    public event EventHandler<OnObjActivatedEventArgs> OnObjActivatedEvent;
    public class OnObjActivatedEventArgs : EventArgs { public GameObject obj; } 
    public void OnObjectiveActivated(GameObject obj)
    {
        OnObjActivatedEvent?.Invoke(this, new OnObjActivatedEventArgs { obj = obj });
    }

    public event EventHandler ScriptedEventNoArgs;
    public void StartScriptedEventNoArgs()
    {
        ScriptedEventNoArgs?.Invoke(this, EventArgs.Empty);
    }

    public event EventHandler<ScriptedEventPlayerArgs> ScriptedEventPlayer;
    public class ScriptedEventPlayerArgs : EventArgs { public GameObject obj; }
    public void StartScriptedEventPlayer(GameObject plr)
    {
        ScriptedEventPlayer?.Invoke(this, new ScriptedEventPlayerArgs { obj = plr });
    }
}
