using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandlerSection3: BaseHandler
{
    [SerializeField] Transform plrStart;

    public override void Restart()
    {
        player.transform.position = plrStart.position;
    }
}
