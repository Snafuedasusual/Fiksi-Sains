using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IScriptedEvents
{
    public enum Triggered
    {
        NotTriggered,
        HasTriggered,
    }

    public void ResetTrigger();

    public void Trigger();
}
