using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityLgcToAnimsComms : LgcToComms
{
    [SerializeField] BaseEnemyLogic enemyLogic;
    public void DeInitializeScript()
    {
        
    }

    public void InitializeScript()
    {
        enemyLogic.EnemyMovementAnimEvent += EnemyMovementAnimEventReceiver;
        enemyLogic.PlayAttackAnimEvent += PlayAttackAnimEventReceiver;
    }

    private void PlayAttackAnimEventReceiver(object sender, BaseEnemyLogic.PlayAttackAnimEventArgs e)
    {
        EnemySendAttackAnimEvent(new EnemyAttackAnimSendEventArgs { anim = e.anim });
    }

    private void EnemyMovementAnimEventReceiver(object sender, BaseEnemyLogic.EnemyMovementAnimEventArgs e)
    {
        EnemySendMovementAnimEvent(e.enemyAnims);
    }

    private void OnEnable()
    {
        InitializeScript();
    }

    private void OnDisable()
    {
        DeInitializeScript();
    }

    private void OnDestroy()
    {
        DeInitializeScript();
    }
}
