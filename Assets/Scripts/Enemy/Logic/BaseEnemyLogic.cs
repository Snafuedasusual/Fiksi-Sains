using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;

public class BaseEnemyLogic : MonoBehaviour, IInflictDamage
{
    [Header("NavMesh Agent")]
    [SerializeField] NavMeshAgent agent;

    [Header("Scriptable Objects")]
    [SerializeField] BaseEnemyStatsSO baseStats;
    [SerializeField] FactionBaseSO baseEnemyFactionsSO;

    [Header("Script References")]
    [SerializeField] BaseSight baseSight;
    [SerializeField] BaseEnemyAlertBar baseEnemyAlertBar;
    [SerializeField] BaseEnemySoundController baseEnemySoundController;
    [SerializeField] EntityHealthController baseEnemyEntityHealthController;

    [Header("Faction")]
    [SerializeField] FactionBaseSO.EnemyFactions currentFaction;

    [Header("Variables")]
    [SerializeField] float defaultSpeed;
    [SerializeField] float defaultAccel;
    [SerializeField] float chaseSpeed;
    [SerializeField] float chaseAccel;
    [SerializeField] float minDistToAttack;
    [SerializeField] Transform target;
    [SerializeField] Transform lastCharToHitMe;
    [SerializeField] Vector3[] targetTrails;
    [SerializeField] Vector3[] patrolRoutes;

    [Header("Idle State Variables")]
    [SerializeField] Vector3 idlePos;
    [SerializeField] Vector3 idleLookRotation;

    [Header("Enemy States")]
    [SerializeField] EnemyStates currentState;
    [SerializeField] EnemyStates defaultState;


    public enum EnemyStates
    {
        None,
        Idle,
        Patrol,
        ChaseTarget,
        Attack,
        ChaseLastKnownPosition,
        LookAroundSearching,
        SearchingAlert,
        Suspicious,
        SuspiciousLookAround,
        SuspiciousApproach,
        SuspiciousRunTowards,
        Dead
    }
    

    private void InitializeEnemy()
    {
        defaultSpeed = baseStats.defaultSpeed;
        defaultAccel = baseStats.defaultAccel;
        chaseSpeed = baseStats.chaseSpeed;
        chaseAccel = baseStats.chaseAccel;
        minDistToAttack = baseStats.minDistToAttack;
        currentFaction = baseEnemyFactionsSO.enemyFactions;

        currentState = defaultState;
    }
    private void Start()
    {
        InitializeEnemy();
        baseSight.SendTarget += SendTargetReceiver;
        baseEnemyAlertBar.AlertBarIsEmpty += AlertBarIsEmptyReceiver;
        baseEnemySoundController.SoundToLogic += SoundToLogicReceiver;
    }

 


    //Handles enemy states.
    private void StateController()
    {
        if(currentState == EnemyStates.Idle)
        {
            Idle();
        }
        if(currentState == EnemyStates.Patrol)
        {
            Patrol();
        }
        if(currentState == EnemyStates.ChaseTarget)
        {
            TrackTrails();
            ChasePlayer();
        }
        if(currentState == EnemyStates.Attack)
        {
            TrackTrails();
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
        if(currentState == EnemyStates.Suspicious)
        {
            Suspicious();
        }
        if(currentState == EnemyStates.SuspiciousApproach)
        {
            SuspiciousApproach();
        }
        if(currentState == EnemyStates.SuspiciousLookAround)
        {
            SuspiciousLookAround();
        }
        if(currentState == EnemyStates.SuspiciousRunTowards)
        {
            SuspiciousRunTowards();
        }
    }
    //Enemy states script ends----------------------------

    //Handles Idle state.
    private void Idle()
    {
        if (transform.position != idlePos)
        {
            agent.destination = idlePos;
            transform.LookAt(transform.position + agent.velocity);
            if (!agent.pathPending)
            {
                if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                {
                    transform.LookAt(idleLookRotation);
                }
            }
        }
        else
        {

        }
    }
    // Idle state script----------------------------------



    // Handles receiving target info
    private void SendTargetReceiver(object sender, BaseSight.SendTargetInfo e)
    {
        if (e.target != null)
        {
            if(lastCharToHitMe == null)
            {
               if(e.target.TryGetComponent(out PlayerLogic plr))
               {
                    if (plr.GetStates() == PlayerLogic.PlayerStates.Hiding || plr.GetStates() == PlayerLogic.PlayerStates.InVent || plr.GetStates() == PlayerLogic.PlayerStates.Dead)
                    {
                        
                    }
                    else
                    {
                        target = e.target;
                        currentState = EnemyStates.ChaseTarget;
                    }
               }
            }
        }
        else if (e.target == null)
        {
            currentState = EnemyStates.ChaseLastKnownPosition;
        }
    }

    private void TargetChecker(Transform trgt)
    {
        if (target != null)
        {
            CheckDistance(target, trgt);
            if(target.TryGetComponent(out PlayerLogic plr))
            {
                if (plr.GetStates() == PlayerLogic.PlayerStates.Hiding || plr.GetStates() == PlayerLogic.PlayerStates.InVent)
                {

                }
                else
                {
                    currentState = EnemyStates.ChaseTarget;
                }
            }
            else if(target.TryGetComponent(out BaseEnemyLogic enemy))
            {
                if(enemy.GetCurrentFaction() == currentFaction)
                {

                }
                else if((enemy.GetCurrentFaction() != currentFaction))
                {
                    currentState = EnemyStates.ChaseTarget;
                }
            }
        }
        else if (target == null)
        {
            if (trgt.TryGetComponent(out PlayerLogic plr))
            {
                if (plr.GetStates() == PlayerLogic.PlayerStates.Hiding || plr.GetStates() == PlayerLogic.PlayerStates.InVent)
                {

                }
                else
                {
                    target = trgt;
                    currentState = EnemyStates.ChaseTarget;
                }
            }
            else if(trgt.TryGetComponent(out BaseEnemyLogic enemy))
            {
                if(enemy.GetCurrentFaction() == currentFaction)
                {

                }
                else if(enemy.GetCurrentFaction() != currentFaction)
                {
                    target = trgt; 
                    currentState = EnemyStates.ChaseTarget;
                }
            }
        }
    }

    private void CheckDistance(Transform target1, Transform target2)
    {
        var distance1 = Vector3.Distance(target1.position, transform.position);
        var distance2 = Vector3.Distance(target2.position, transform.position);

        if (distance1 < distance2)
        {
            target = target1;
        }
        else if (distance1 > distance2)
        {
            target = target2;
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
        if (!agent.pathPending)
        {
            if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
            {
                SendInfoToAlertBar(0f);
                currentState = EnemyStates.LookAroundSearching;
            }
        }
    }
    // Chase last known position script ends-----------------



    // Handles Attacking target state and events related.
    public event EventHandler<OnAttackEventArgs> OnAttackEvent;
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
        lastCharToHitMe = null;
        if (IsLookingAroundSearching == null)
        {
            SendInfoToAlertBar(0f);
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

    int indexPatrol = 0;
    private void Patrol()
    {
        if(patrolRoutes.Length > 1)
        {
            agent.isStopped = false;
            agent.speed = defaultSpeed;
            agent.acceleration = defaultAccel;
            agent.destination = patrolRoutes[indexPatrol];
            transform.LookAt(transform.position + agent.velocity);
            var distance = Vector3.Distance(transform.position, patrolRoutes[indexPatrol]);
            if (distance < 0.70)
            {
                if (indexPatrol < patrolRoutes.Length - 1)
                {
                    indexPatrol++;
                }
                else if (indexPatrol == patrolRoutes.Length - 1)
                {
                    indexPatrol = 0;
                }
            }
        } 
    }

    private void Suspicious()
    {
        if(IsSuspicious == null)
        {
            IsSuspicious = StartSuspicious();
            StartCoroutine(IsSuspicious);
        }
    }
    IEnumerator IsSuspicious;
    IEnumerator StartSuspicious()
    {
        var lookSpeed = 10f;
        var delayTime = 0f;
        var delayRate = 1.2f;
        var target = new Vector3(location.x, transform.position.y, location.z);
        var lookDir = Quaternion.LookRotation(target - transform.position);
        if (currentState != EnemyStates.Suspicious)
        {
            StopCoroutine(IsSuspicious);
            IsSuspicious = null;
        }
        else
        {
            while(delayTime < delayRate)
            {
                if(currentState != EnemyStates.Suspicious)
                {
                    StopCoroutine(IsSuspicious);
                    IsSuspicious = null;

                }
                else
                {
                    transform.rotation = Quaternion.Slerp(transform.rotation, lookDir, lookSpeed * Time.deltaTime);
                    delayTime += Time.deltaTime;
                    yield return 0;
                }
                
            }
            currentState = EnemyStates.SuspiciousApproach;
        }
        IsSuspicious = null;
    }


    private void SuspiciousApproach()
    {
        if(location == Vector3.zero)
        {

        }
        else
        {
            agent.destination = location;
            agent.speed = defaultSpeed;
            agent.acceleration = defaultAccel;
            transform.LookAt(agent.velocity + transform.position);
            var distance = Vector3.Distance(location, transform.position);
            if (distance < 0.5f)
            {
                agent.velocity = Vector3.zero;
                currentState = EnemyStates.SuspiciousLookAround;
            }
        }
    }

    private void SuspiciousLookAround()
    {
        if(IsSuspiciousLooking == null)
        {
            IsSuspiciousLooking = StartSuspiciousLookAround();
            StartCoroutine(IsSuspiciousLooking);
        }
    }
    IEnumerator IsSuspiciousLooking;
    IEnumerator StartSuspiciousLookAround()
    {
        var amountOfLooks = 3;
        var lookCount = 0;
        var lookTime = 0f;
        var lookRate = 2f;
        if (currentState != EnemyStates.SuspiciousLookAround)
        {
            StopCoroutine(IsSuspiciousLooking);
            lookCount = 0;
            IsSuspiciousLooking = null;
        }
        else
        {
            while (currentState == EnemyStates.SuspiciousLookAround && lookCount < amountOfLooks)
            {
                var randomDirX = Random.Range(-1f, 1f);
                var randomDirZ = Random.Range(-1f, 1f);
                var lookSpeed = 10f;
                var direction = new Vector3(randomDirX + transform.position.x, transform.position.y, randomDirZ + transform.position.z);
                var lookDir = Quaternion.LookRotation(direction - transform.position);
                lookTime = 0f;
                while (lookTime < lookRate)
                {
                    if(currentState != EnemyStates.SuspiciousLookAround)
                    {
                        StopCoroutine(IsSuspiciousLooking);
                        lookCount = 0;
                        IsLookingAroundSearching = null;
                    }
                    else
                    {
                        transform.rotation = Quaternion.Slerp(transform.rotation, lookDir, lookSpeed * Time.deltaTime);
                        lookTime += Time.deltaTime;
                    }
                    yield return 0;
                }
                if (lookCount < amountOfLooks)
                {
                    lookCount++;
                }
                if (currentState != EnemyStates.SuspiciousLookAround)
                {
                    StopCoroutine(IsSuspiciousLooking);
                    IsSuspiciousLooking = null;
                    lookCount = 0;
                }
                else if (lookCount >= amountOfLooks)
                {
                    StopCoroutine(IsSuspiciousLooking);
                    IsSuspiciousLooking = null;
                    currentState = defaultState;
                    lookCount = 0;
                }
            }
        }
    }


    private void SuspiciousRunTowards()
    {
        if (location == Vector3.zero)
        {

        }
        else
        {
            agent.destination = location;
            agent.speed = chaseSpeed;
            agent.acceleration = chaseAccel;
            transform.LookAt(agent.velocity + transform.position);
            if (!agent.pathPending)
            {
                if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                {
                    SendInfoToAlertBar(0f);
                    currentState = EnemyStates.LookAroundSearching;
                }

            }
        }
    }


    private void TrackTrails()
    {
        if(IsTrackingTrails == null)
        {
            IsTrackingTrails = StartTrackingTrails();
            StartCoroutine(IsTrackingTrails);
        }
    }
    IEnumerator IsTrackingTrails;
    IEnumerator StartTrackingTrails()
    {
        var trackTime = 0f;
        var trackRate = 0.1f;
        var trackAmounts = 120;
        var trackCount = 0;
        if(target != null)
        {
            while(target != null)
            {
                trackTime = 0f;
                while(trackTime < trackRate)
                {
                    if(target == null)
                    {
                        StopCoroutine(IsTrackingTrails);
                        IsTrackingTrails = null;
                    }
                    else
                    {
                        trackTime += Time.deltaTime * 15f;
                    }
                    yield return 0;
                }
                if(target == null)
                {
                    StopCoroutine(IsTrackingTrails);
                    IsTrackingTrails = null;
                }
                else if(target != null)
                {
                    AddTrails(target.position);
                    if (trackCount < trackAmounts)
                    {
                        trackCount++;
                    }
                    else if(trackCount >= trackAmounts)
                    {
                        StopCoroutine(IsTrackingTrails);
                        IsTrackingTrails = null;
                    }
                }
            }
        }
        else
        {

        }
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
    private Vector3 GetLastSeenPosition()
    {
        var minDist = 10f;
        var lastPos = Vector3.zero;
        if(target != null)
        {
            for (int i = 0; i < targetTrails.Length; i++)
            {
                var distance = Vector3.Distance(target.position, targetTrails[i]);
                if(distance < minDist)
                {
                    minDist = distance;
                    lastPos = targetTrails[i];
                }
                else
                {

                }
            }
        }
        return lastPos;
    }
    //Getting last seen position script ends---------------------




    public event EventHandler<SendEventToAlertBarScrArgs> SendEventToAlertBarScr;
    public class SendEventToAlertBarScrArgs : EventArgs { public float adder; }
    private void SendInfoToAlertBar(float adder)
    {
        SendEventToAlertBarScr?.Invoke(this, new SendEventToAlertBarScrArgs { adder = adder });
    }


    
    //Handles Alert bar communications.
    private void AlertBarIsEmptyReceiver(object sender, EventArgs e)
    {
        AlertBarIsEmpty();
    }
    private void AlertBarIsEmpty()
    {
        StopAllCoroutines();
        lastCharToHitMe = null;
        currentState = defaultState;
        target = null;
    }
    //Alert bar communication script ends-----------------------------


    private void SoundToLogicReceiver(object sender, BaseEnemySoundController.SoundToLogicArgs e)
    {
        ReceivingAudioInfo(e.alertLevel, e.location);
    }
    Vector3 location;
    private void ReceivingAudioInfo(float alertLevel, Vector3 target)
    {
        var lowAlertLevel = 1f;
        var highAlertLevel = 2f;
        var knowLocation = 3f;
        if (currentState == EnemyStates.ChaseTarget || currentState == EnemyStates.Attack || currentState == EnemyStates.ChaseLastKnownPosition || currentState == EnemyStates.None || currentState == EnemyStates.SuspiciousRunTowards)
        {

        }
        else
        {
            if(alertLevel == lowAlertLevel)
            {
                currentState = EnemyStates.Suspicious;
                location = target;
            }
            else if(alertLevel == highAlertLevel)
            {
                SendInfoToAlertBar(30f);
                currentState = EnemyStates.SuspiciousRunTowards;
                location = target;
            }
        }
    }

    public FactionBaseSO.EnemyFactions GetCurrentFaction()
    {
        return currentFaction;
    }

    private void Update()
    {
        StateController();
    }

    private void OnDisable()
    {
        baseSight.SendTarget -= SendTargetReceiver;
        baseEnemyAlertBar.AlertBarIsEmpty -= AlertBarIsEmptyReceiver;
        baseEnemySoundController.SoundToLogic -= SoundToLogicReceiver;
    }

    private void OnDestroy()
    {
        baseSight.SendTarget -= SendTargetReceiver;
        baseEnemyAlertBar.AlertBarIsEmpty -= AlertBarIsEmptyReceiver;
        baseEnemySoundController.SoundToLogic -= SoundToLogicReceiver;
    }
}
