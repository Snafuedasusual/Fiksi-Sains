using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalHandler : BaseHandler
{
    [SerializeField] Transform plrStart;
    [SerializeField] Door_Section end;
    public override void Restart()
    {
        player.transform.position = plrStart.position;
    }

    public void ActivateEnd()
    {
        end.canFinish = true;
    }
}
