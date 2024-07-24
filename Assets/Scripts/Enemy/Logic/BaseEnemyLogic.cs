using System;
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
        Idle,
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

    private void StateController()
    {
        if(currentState == EnemyStates.ChasePlayer)
        {
            ChasePlayer();
        }
        if(currentState == EnemyStates.Attack)
        {
            Attack();
        }
        if(currentState == EnemyStates.ChaseLastKnownPosition)
        {
            ChaseLastKnown(GetLastSeenPosition());
        }
    }

    // Handles receiving target info
    private void SendTargetReceiver(object sender, BaseSight.SendTargetInfo e)
    {
        if(e.target != null)
        {
            if (e.target.TryGetComponent(out PlayerLogic plr))
            {
                if (plr.GetStates() == PlayerLogic.PlayerStates.Hiding || plr.GetStates() == PlayerLogic.PlayerStates.InVent)
                {
                    
                }
                else
                {
                    target = e.target;
                    currentState = EnemyStates.ChasePlayer;
                }
            } 
        }
        else
        {
            currentState = EnemyStates.ChaseLastKnownPosition;
        }
    }
    // Script handling target info ends------------------------------

    private void ChasePlayer()
    {
        agent.destination = target.position;
        agent.speed = chaseSpeed;
        agent.acceleration = chaseAccel;
        agent.isStopped = false;
        transform.LookAt(new Vector3(target.position.x, transform.position.y, target.position.z));
        var distance = Vector3.Distance(target.position, transform.position);
        if(distance < minDistToAttack)
        {
            agent.velocity = Vector3.zero;
            agent.isStopped = true;
            currentState = EnemyStates.Attack;
        }
        else
        {
            agent.isStopped = false;
        }
    }

    private void ChaseLastKnown(Vector3 pos)
    {
        agent.destination = pos;
        var distance = Vector3.Distance(pos, transform.position);
        if(transform.position == pos)
        {
            agent.velocity = Vector3.zero;
            agent.isStopped = true;
        }
    }


    public event EventHandler OnAttackEvent;
    public class OnAttackEventArgs : EventArgs { public Transform target; }
    private void Attack()
    {
        var distance = Vector3.Distance(target.position, transform.position);
        if (target == null)
        {

        }
        else if (target != null && distance < minDistToAttack)
        {
            transform.LookAt(new Vector3(target.position.x, transform.position.y, target.position.z));
            OnAttackEvent?.Invoke(this, new OnAttackEventArgs { target = target });
        }
        else
        {
            
        }
    }


    // Handles when Player trails is added.
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
    // Script Player trails ends--------------------------------

    [SerializeField] Vector3 targetPos;
    private Vector3 GetLastSeenPosition()
    {
        var minDist = 10f;
        if(target != null)
        {
            for (int i = 0; i < targetTrails.Length; i++)
            {
                var distance = Vector3.Distance(targetTrails[i], target.position);
                if(distance < minDist)
                {
                    minDist = distance;
                    targetPos = targetTrails[i];
                }
            }
        }
        return targetPos;
    }

    private void Update()
    {
        StateController();
    }
}
