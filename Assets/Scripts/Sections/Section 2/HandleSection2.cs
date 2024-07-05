using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandlerSection2 : BaseHandler, ILogicSection
{
    [Header("StartLogic")]
    [SerializeField] Transform plrStart;
    [SerializeField] Vector3[] deadScientistLocation;
    [SerializeField] Transform deadScientist;
    [SerializeField] Door_Section Section;
 
    public override void Restart()
    {
        deadScientist.localPosition = deadScientistLocation[Random.Range(0, deadScientistLocation.Length - 1)];
        player.transform.position = plrStart.position;
    }

    public void CanFinish()
    {
        Section.canFinish = true;
    }

    private void Start()
    {
        deadScientist.localPosition = deadScientistLocation[Random.Range(0, deadScientistLocation.Length - 1)];
    }
}
