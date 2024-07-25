using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;

public class BaseEnemyLogic : MonoBehaviour
{
    [Header("NavMesh Agent")]
    [SerializeField] NavMeshAgent agent;

    [Header("Scriptable Objects")]
    [SerializeField] BaseEnemyStatesSO baseStats;

    [Header("Script References")]
    [SerializeField] BaseSight baseSight;
    [SerializeField] BaseEnemyAlertBar baseEnemyAlertBar;

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
    [SerializeField] EnemyStates defaultState;
    public enum EnemyStates
    {
        Idle,
        ChasePlayer,
        Attack,
        ChaseLastKnownPosition,
        LookAroundSearching,
        SearchingAlert,
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
        baseEnemyAlertBar.AlertBarIsEmpty += AlertBarIsEmptyReceiver;
    }


    //Handles enemy states.
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
        if(currentState == EnemyStates.LookAroundSearching)
        {
            LookAroundSearching();
        }
        if(currentState == EnemyStates.SearchingAlert)
        {
            SearchingAlert(RandomPointToSearch());
        }
    }
    //Enemy states script ends----------------------------



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



    // Handles chase player state.
    private void ChasePlayer()
    {
        SendInfoToAlertBar(20f);
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
    // Chase player script ends---------------------------------



    // Handles Chase last known position state.
    private void ChaseLastKnown(Vector3 pos)
    {
        agent.destination = pos;
        var distance = Vector3.Distance(pos, transform.position);
        transform.LookAt(agent.velocity + transform.position);
        if (distance < 0.75)
        {
            SendInfoToAlertBar(0f);
            currentState = EnemyStates.LookAroundSearching;
        }
        else
        {

        }
    }
    // Chase last known position script ends-----------------



    // Handles Attacking target state and events related.
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
    //Attacking script ends---------------------------------------




    //Handles looking around when searching state.
    private void LookAroundSearching()
    {
        if(IsLookingAroundSearching == null)
        {
            IsLookingAroundSearching = LookingAroundAndSearch();
            StartCoroutine(IsLookingAroundSearching);
        }
        else
        {

        }
    }
    IEnumerator IsLookingAroundSearching;
    IEnumerator LookingAroundAndSearch()
    {
        var amountOfLooks = 6;
        var lookCount = 0;
        var lookTime = 0f;
        var lookRate = 1.5f;
        if(currentState != EnemyStates.LookAroundSearching)
        {
            Debug.Log("Coroutine Stop");
            StopCoroutine(IsLookingAroundSearching);
            lookCount = 0;
            IsLookingAroundSearching = null;
        }
        else
        {
            while(currentState == EnemyStates.LookAroundSearching && lookCount < amountOfLooks)
            {
                var randomDirX = Random.Range(-1f, 1f);
                var randomDirZ = Random.Range(-1f, 1f);
                var lookSpeed = Random.Range(4f, 10f);
                var direction = new Vector3(randomDirX + transform.position.x, transform.position.y, randomDirZ + transform.position.z);
                var lookDir = Quaternion.LookRotation(direction - transform.position);
                lookTime = 0f;
                while (lookTime < lookRate)
                {
                    transform.rotation = Quaternion.Slerp(transform.rotation, lookDir, lookSpeed * Time.deltaTime);
                    lookTime += Time.deltaTime;
                    yield return 0;
                }
                if(lookCount < amountOfLooks)
                {
                    lookCount++;
                }
                if(currentState != EnemyStates.LookAroundSearching)
                {
                    StopCoroutine(IsLookingAroundSearching);
                    IsLookingAroundSearching = null;
                    lookCount = 0;
                }
                else if(lookCount >= amountOfLooks)
                {
                    StopCoroutine(IsLookingAroundSearching);
                    IsLookingAroundSearching = null;
                    currentState = EnemyStates.SearchingAlert;
                    lookCount = 0;
                }
            }
        }
    }
    //Looking around while searching state script ends-------------------



    //Handles Searching while alert state.
    private void SearchingAlert(Vector3 hitpoint)
    {
        var point = hitpoint;
        var distance = Vector3.Distance(point, transform.position);
        if(distance > 7.5f)
        {
            if (IsSearchingAlert == null)
            {
                IsSearchingAlert = StartSearchingAlert(point);
                StartCoroutine(IsSearchingAlert);
            }
            else
            {

            }
        }
        else
        {

        }
    }
    private Vector3 RandomPointToSearch()
    {
        var radius = 20f;
        var randomPoint = Random.insideUnitSphere * radius;
        randomPoint += transform.position;
        var targetPos = Vector3.zero;
        if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, radius, NavMesh.AllAreas))
        {
            targetPos = hit.position;
        }
        else
        {

        }
        return targetPos;
    }
    IEnumerator IsSearchingAlert;
    IEnumerator StartSearchingAlert(Vector3 point)
    {
        
        if(currentState != EnemyStates.SearchingAlert)
        {

        }
        else
        {
            
            while (currentState == EnemyStates.SearchingAlert)
            {
                var distance = Vector3.Distance(point, transform.position);
                agent.destination = point;
                agent.speed = defaultSpeed;
                agent.acceleration = defaultAccel;
                transform.LookAt(agent.velocity + transform.position);
                if(distance < 0.75)
                {
                    currentState = EnemyStates.LookAroundSearching;
                    StopCoroutine(IsSearchingAlert);
                    IsSearchingAlert = null;
                    break;
                }
                if(currentState != EnemyStates.SearchingAlert)
                {
                    StopCoroutine(IsSearchingAlert);
                    IsSearchingAlert = null;
                    break;
                }
                else
                {

                }
                yield return null;
            }
        }
    }
    //Searching target while alert script ends-----------------------



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



    //Handles Getting last seen position.
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
    //Getting last seen position script ends---------------------

    public event EventHandler<SendEventToAlertBarScrArgs> SendEventToAlertBarScr;
    public class SendEventToAlertBarScrArgs : EventArgs { public float adder; }
    private void SendInfoToAlertBar(float adder)
    {
        SendEventToAlertBarScr?.Invoke(this, new SendEventToAlertBarScrArgs { adder = adder });
    }

    private void AlertBarIsEmptyReceiver(object sender, EventArgs e)
    {
        AlertBarIsEmpty();
    }
    private void AlertBarIsEmpty()
    {
        StopAllCoroutines();
        currentState = defaultState;
    }

    private void Update()
    {
        StateController();
    }
}
