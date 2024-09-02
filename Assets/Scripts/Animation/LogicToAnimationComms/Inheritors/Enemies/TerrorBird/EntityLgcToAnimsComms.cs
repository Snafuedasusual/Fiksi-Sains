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
    }

    private void EnemyMovementAnimEventReceiver(object sender, BaseEnemyLogic.EnemyMovementAnimEventArgs e)
    {
        EnemyMovement(e.enemyAnims);
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
