using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandlerSection1 : BaseHandler, ILogicSection
{

    [Header("StartLogic")]
    [SerializeField] Transform plrStart;

    [Header("Scriptable Events")]
    [SerializeField] Activation_Object activator;
    [SerializeField] Door_Section doorSection;

    public void UnlockDoorSection()
    {
        doorSection.canFinish = true;
    }

    public override void Restart()
    {
        player.transform.position = plrStart.position;
    }
}
