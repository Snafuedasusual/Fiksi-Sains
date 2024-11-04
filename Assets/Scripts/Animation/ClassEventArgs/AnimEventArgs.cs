using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimEventArgs : EventArgs
{
    public MoveStatesSO.MoveStates moveState;
    public BaseEnemyLogic.EnemyStates enemyState;
    public PlayerLogic.PlayerStates playerState;
}
