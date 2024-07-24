using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;

public class BaseEnemyLogic : MonoBehaviour
{
    [Header("NavMesh Agent")]
    [SerializeField] NavMeshAgent agent;

    [Header("Scriptable Objects")]
    [SerializeField] BaseEnemyStatesSO baseStats;

    [Header("Script References")]
    [SerializeField] BaseSight baseSight;

    [Header("Variables")]
    [SerializeField] float defaultSpeed;
    [SerializeField] float defaultAccel;
    [SerializeField] float chaseSpeed;
    [SerializeField] float chaseAccel;
    [SerializeField] float minDistToAttack;
    [SerializeField] Transform target;
    [SerializeField] Vector3[] targetTrails;

    [Header("Enemy States")]
    [SerializeField] EnemyStates currentState;
    public enum EnemyStates
    {
        ChasePlayer,
        Attack,
        ChaseLastKnownPosition,
        Dead
    }
    

    private void InitializeEnemy()
    {
        defaultSpeed = baseStats.defaultSpeed;
        defaultAccel = baseStats.defaultAccel;
        chaseSpeed = baseStats.chaseSpeed;
        chaseAccel = baseStats.chaseAccel;
        minDistToAttack = baseStats.minDistToAttack;
    }
    private void Start()
    {
        InitializeEnemy();
        baseSight.SendTarget += SendTargetReceiver;
        baseSight.SendTargetPos += SendTargetPosReceiver;
    }



    private void SendTargetReceiver(object sender, BaseSight.SendTargetInfo e)
    {
        if(e.target != null)
        {
            target = e.target;
            ChasePlayer();
        }
        else
        {
            ChaseLastKnown();
        }
    }

    private void ChasePlayer()
    {

    }

    private void ChaseLastKnown()
    {

    }


    private void SendTargetPosReceiver(object sender, BaseSight.SendTargetPosArgs e)
    {
        AddTrails(e.target);
    }
    int trailIndex = 0;
    private void AddTrails(Vector3 newTrails)
    {
        targetTrails[trailIndex] = newTrails;
        if(trailIndex < targetTrails.Length - 1)
        {
            trailIndex++;
        }
        else if(trailIndex == targetTrails.Length - 1)
        {
            trailIndex = 0;
        }
    }
}
