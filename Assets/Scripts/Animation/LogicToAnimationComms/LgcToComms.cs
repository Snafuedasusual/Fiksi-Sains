using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PlayerLogic;

public class LgcToComms : MonoBehaviour
{


    public event EventHandler<EnemyMovementAnimSendEventArgs> EnemyMovementAnimSendEvent;
    public class EnemyMovementAnimSendEventArgs : EventArgs { public BaseEnemyLogic.EnemyAnimations anim; }
    
    protected void EnemyMovement(BaseEnemyLogic.EnemyAnimations newAnim)
    {
        var handler = EnemyMovementAnimSendEvent;
        if(handler != null)
        {
            handler?.Invoke(this, new EnemyMovementAnimSendEventArgs { anim = newAnim});
        }
    }


    protected virtual void EnemySendAnimEvents(BaseEnemyLogic.EnemyAnimEventArgs e)
    {

    }









    public event EventHandler PlayerSendAnimEvent;
    public class PlayerSendEventArgs : EventArgs { public PlayerLogic.PlrAnimations anim; }
    public virtual void PlayerSendAnim()
    {

    }

    public event EventHandler<PlayerSendMovementAnimEventArgs> PlayerMovementAnimEvent;
    public class PlayerSendMovementAnimEventArgs : EventArgs { public PlrAnimations playThisAnim; public float xAxis; public float yAxis; }
    protected void PlayerSendMovementAnimEvent(PlayerSendMovementAnimEventArgs sendArgs)
    {
        var handler = PlayerMovementAnimEvent;
        if (handler != null)
        {
            handler?.Invoke(this, sendArgs);
        }
    }
}
