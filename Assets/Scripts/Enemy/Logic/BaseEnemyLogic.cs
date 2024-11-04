using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;

public class BaseEnemyLogic : MonoBehaviour, IInitializeScript, IKnockBack
{
    [Header("NavMesh Agent")]
    [SerializeField] protected NavMeshAgent agent;

    [Header("Scriptable Objects")]
    [SerializeField] protected BaseEnemyStatsSO baseStats;
    [SerializeField] protected FactionBaseSO baseEnemyFactionsSO;

    [Header("Script References")]
    [SerializeField] protected BaseSight baseSight;
    [SerializeField] protected BaseEnemyAlertBar baseEnemyAlertBar;
    [SerializeField] protected BaseEnemySoundController baseEnemySoundController;
    [SerializeField] protected EntityHealthController baseEnemyEntityHealthController;
    [SerializeField] protected AnimEventArgs AnimEventArgs;

    [Header("Visuals")]
    [SerializeField] GameObject visuals;
    [SerializeField] float centerBody;
    [SerializeField] GameObject specialActor;

    [SerializeField] RuntimeAnimatorController defaultSAController;
    [SerializeField] RuntimeAnimatorController overrideSAController;

    [Header("Components")]
    [SerializeField] Rigidbody rb;

    [Header("Faction")]
    [SerializeField] FactionBaseSO.EnemyFactions currentFaction;

    [Header("Variables")]
    [SerializeField] float defaultSpeed;
    [SerializeField] float defaultAccel;
    [SerializeField] float chaseSpeed;
    [SerializeField] float chaseAccel;
    [SerializeField] float minDistToAttack;
    [SerializeField] protected Transform target;
    [SerializeField] protected Transform lastCharToHitMe;
    [SerializeField] protected Vector3[] targetTrails;
    [SerializeField] protected Vector3[] patrolRoutes;
    [SerializeField] protected LayerMask obstacles;

    [Header("Idle State Variables")]
    [SerializeField] protected Vector3 idlePos;
    [SerializeField] protected Vector3 idleLookRotation;

    [Header("Enemy States")]
    [SerializeField] protected EnemyStates currentState;
    [SerializeField] protected EnemyStates defaultState;

    [Header("MoveStates")]
    [SerializeField] protected MoveStatesSO.MoveStates currentMoveState;


    [SerializeField] protected Vector3 destinasion;

    public event EventHandler<EnemyAnimEventArgs> EnemyAnimEvent;
    public class EnemyAnimEventArgs : EventArgs { public EnemyStates currentState; public MoveStatesSO.MoveStates currentMoveState; }

    public event EventHandler<EnemyMovementAnimEventArgs> EnemyMovementAnimEvent;
    public class EnemyMovementAnimEventArgs : EventArgs { public EnemyAnimations enemyAnims; }


    public enum EnemyStates
    {
        Null,
        FR_Idle,
        FR_Patrol,
        ChaseTarget,
        Attack,
        ChaseLastKnownPosition,
        LookAroundSearching,
        SearchingAlert,
        FR_Wandering,
        FR_LookAround,
        Suspicious,
        SuspiciousLookAround,
        SuspiciousApproach,
        SuspiciousRunTowards,
        Dead
    }
    
    public enum EnemyAnimations
    {
        IDLE,
        WALK,
        RUN,
        ATTACK,
        NONE,
    }

    public virtual void InitializeEnemy()
    {
        defaultSpeed = baseStats.defaultSpeed;
        defaultAccel = baseStats.defaultAccel;
        chaseSpeed = baseStats.chaseSpeed;
        chaseAccel = baseStats.chaseAccel;
        minDistToAttack = baseStats.minDistToAttack;
        currentFaction = baseEnemyFactionsSO.enemyFactions;
        currentMoveState = MoveStatesSO.MoveStates.Idle;
        currentState = defaultState;
    }

    public void InitializeScript()
    {
        baseSight.SendTarget += SendTargetReceiver;
        baseEnemyAlertBar.AlertBarIsEmpty += AlertBarIsEmptyReceiver;
        baseEnemySoundController.SoundToLogic += SoundToLogicReceiver;
        baseEnemyEntityHealthController.SendDmgToLogic += SendDmgToLogicReceiver;
    }


    public void DeInitializeScript()
    {
        baseSight.SendTarget -= SendTargetReceiver;
        baseEnemyAlertBar.AlertBarIsEmpty -= AlertBarIsEmptyReceiver;
        baseEnemySoundController.SoundToLogic -= SoundToLogicReceiver;
        baseEnemyEntityHealthController.SendDmgToLogic -= SendDmgToLogicReceiver;
    }

    private void Start()
    {
        InitializeEnemy();
        InitializeScript();
    }

    private void OnEnable()
    {
        InitializeEnemy();
        InitializeScript();
        DelayAudioPlay = null;
        IsLookingAroundSearching = null;
        lastCharToHitMe = null;
        IsSearchingAlert = null;
        IsSuspicious = null;
        IsSuspiciousLooking = null;
    }

    private void CalculateCenterBody()
    {
        if(Time.frameCount % 10 == 0)
        {
            centerBody = (visuals.transform.position.y - transform.position.y) / 2;
        }
    }


    //Handles enemy states.
    public virtual void StateController()
    {
        if(currentState == EnemyStates.Null)
        {
            
        }
        if(currentState == EnemyStates.FR_Idle)
        {
            Idle();
        }
        if(currentState == EnemyStates.FR_Patrol)
        {
            Patrol();
        }
        if(currentState == EnemyStates.FR_Wandering)
        {
            FR_WanderingAround(RandomPointToSearch());
        }
        if(currentState == EnemyStates.FR_LookAround)
        {
            FR_LookAround();
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
    public virtual void Idle()
    {
        var distance = Vector3.Distance(idlePos, transform.position);
        if (distance > agent.stoppingDistance)
        {
            WalkToPos(idlePos);
            destinasion = agent.destination;
            transform.LookAt(transform.position + agent.velocity);
        }
        else if (distance <= agent.stoppingDistance)
        {
            transform.LookAt(idleLookRotation);
            StopMove();
        }
    }
    // Idle state script----------------------------------


    private void RunToPos(Vector3 pos)
    {
        agent.isStopped = false;
        agent.destination = pos;
        agent.acceleration = chaseAccel;
        agent.speed = chaseSpeed;
        currentMoveState = MoveStatesSO.MoveStates.Run;
        //EnemyAnimEvent?.Invoke(this, new EnemyAnimEventArgs { currentState = currentState, currentMoveState = currentMoveState});
        EnemyMovementAnimEvent?.Invoke(this, new EnemyMovementAnimEventArgs { enemyAnims = EnemyAnimations.RUN });
    }

    private void WalkToPos(Vector3 pos)
    {
        agent.isStopped = false;
        agent.destination = pos;
        agent.acceleration = defaultAccel;
        agent.speed = defaultSpeed;
        currentMoveState = MoveStatesSO.MoveStates.Walk;
        //EnemyAnimEvent?.Invoke(this, new EnemyAnimEventArgs { currentState = currentState, currentMoveState = currentMoveState });
        EnemyMovementAnimEvent?.Invoke(this, new EnemyMovementAnimEventArgs { enemyAnims = EnemyAnimations.WALK });
    }

    private void StopMove()
    {
        agent.velocity = Vector3.zero;
        agent.isStopped = true;
        currentMoveState = MoveStatesSO.MoveStates.Idle;
        //EnemyAnimEvent?.Invoke(this, new EnemyAnimEventArgs { currentState = currentState, currentMoveState = currentMoveState });
        EnemyMovementAnimEvent?.Invoke(this, new EnemyMovementAnimEventArgs { enemyAnims = EnemyAnimations.IDLE });
    }




    // Handles receiving target info
    public virtual void SendTargetReceiver(object sender, BaseSight.SendTargetInfo e)
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

    public virtual void TargetChecker(Transform trgt)
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

    public virtual void CheckDistance(Transform target1, Transform target2)
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
    public virtual void ChasePlayer()
    {
        if(target != null)
        {
            AddEnemyToAlertList();
            SendInfoToAlertBar(20f);
            RunToPos(target.position);
            transform.LookAt(new Vector3(target.position.x, transform.position.y, target.position.z));
            var distance = Vector3.Distance(target.position, transform.position);
            if (distance < minDistToAttack)
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
        else
        {
            currentState = defaultState;
        }
        
    }
    // Chase player script ends---------------------------------



    // Handles Chase last known position state.
    public virtual void ChaseLastKnown(Vector3 pos)
    {
        agent.destination = pos;
        RunToPos(pos);
        var distance = Vector3.Distance(pos, transform.position);
        transform.LookAt(agent.velocity + transform.position);
        if (!agent.pathPending)
        {
            if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
            {
                SendInfoToAlertBar(0f);
                StopMove();
                currentState = EnemyStates.LookAroundSearching;
            }
        }
    }
    // Chase last known position script ends-----------------



    // Handles Attacking target state and events related.
    public event EventHandler<OnAttackEventArgs> OnAttackEvent;
    public class OnAttackEventArgs : EventArgs { public Transform sender; public Transform target; }
    public virtual void Attack()
    {
        
        var distance = Vector3.Distance(target.position, transform.position);
        if (target == null)
        {

        }
        else if (target != null && distance < minDistToAttack)
        {
            StopMove();
            transform.LookAt(new Vector3(target.position.x, transform.position.y, target.position.z));
            OnAttackEvent?.Invoke(this, new OnAttackEventArgs { sender = transform, target = target });
        }
        else
        {
            currentState = EnemyStates.ChaseTarget;
        }
    }
    //Attacking script ends---------------------------------------

    //public event EventHandler
    public event EventHandler<PlayAttackAnimEventArgs> PlayAttackAnimEvent;
    public class PlayAttackAnimEventArgs : EventArgs { public EnemyAnimations anim; }
    public void PlayAttackAnim()
    {
        PlayAttackAnimEvent?.Invoke(this, new PlayAttackAnimEventArgs { anim = EnemyAnimations.ATTACK });
    }



    //Handles looking around when searching state.
    public virtual void LookAroundSearching()
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
    protected IEnumerator IsLookingAroundSearching;
    public virtual IEnumerator LookingAroundAndSearch()
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
                var lookSpeed = Random.Range(2f, 10f);
                var direction = new Vector3(randomDirX + transform.position.x, transform.position.y, randomDirZ + transform.position.z);
                var lookDir = Quaternion.LookRotation(direction - transform.position);
                lookTime = 0f;
                while (lookTime < lookRate)
                {
                    if(currentState != EnemyStates.LookAroundSearching)
                    {
                        StopCoroutine(IsLookingAroundSearching);
                        IsLookingAroundSearching = null;
                    }
                    else if(currentState == EnemyStates.LookAroundSearching)
                    {
                        transform.rotation = Quaternion.Slerp(transform.rotation, lookDir, lookSpeed * Time.deltaTime);
                        lookTime += Time.deltaTime;
                    }
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
                if(lookCount >= amountOfLooks)
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


    protected virtual void FR_LookAround()
    {
        if (LookAround != null) return;
        LookAround = StartCoroutine(StartLookAround());
    }

    protected Coroutine LookAround;
    protected IEnumerator StartLookAround()
    {
        var amountOfLooks = 3;
        var lookCount = 0;
        var lookTime = 0f;
        var lookRate = 2f;
        if (currentState != EnemyStates.FR_LookAround) { LookAround = null; yield break; }
        while(currentState == EnemyStates.FR_LookAround && lookCount < amountOfLooks)
        {
            var randomDirX = Random.Range(-1f, 1f);
            var randomDirZ = Random.Range(-1f, 1f);
            var lookSpeed = Random.Range(2f, 10f);
            var direction = new Vector3(randomDirX + transform.position.x, transform.position.y, randomDirZ + transform.position.z);
            var lookDir = Quaternion.LookRotation(direction - transform.position);
            Debug.Log(lookCount);
            while (lookTime < lookRate)
            {
                if (currentState != EnemyStates.FR_LookAround) { LookAround = null; yield break; }
                else if (currentState == EnemyStates.FR_LookAround)
                {
                    transform.rotation = Quaternion.Slerp(transform.rotation, lookDir, lookSpeed * Time.deltaTime);
                    lookTime += Time.deltaTime;
                }
                yield return null;
            }
            lookTime = 0f;
            if(lookCount < amountOfLooks) { Debug.Log("NotYetEnd"); lookCount++; lookTime = 0f; }
            if (currentState != EnemyStates.FR_LookAround) { Debug.Log("AbruptEnd"); LookAround = null; yield break; }
            if(lookCount >= amountOfLooks) { Debug.Log("End"); LookAround = null; currentState = EnemyStates.FR_Wandering; yield break; }
            yield return null;
        }
    }


    //Handles Searching while alert state.
    public virtual void SearchingAlert(Vector3 hitpoint)
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
    public virtual Vector3 RandomPointToSearch()
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


    protected IEnumerator IsSearchingAlert;
    public virtual IEnumerator StartSearchingAlert(Vector3 point)
    {
        
        if(currentState != EnemyStates.SearchingAlert)
        {
            IsSearchingAlert = null;
            yield break;
        }
        else
        {
            
            while (currentState == EnemyStates.SearchingAlert)
            {
                var distance = Vector3.Distance(point, transform.position);
                WalkToPos(point);
                destinasion = agent.destination;
                transform.LookAt(agent.velocity + transform.position);
                if(distance < agent.stoppingDistance)
                {
                    currentState = EnemyStates.LookAroundSearching;
                    StopCoroutine(IsSearchingAlert);
                    IsSearchingAlert = null;
                    StopMove();
                    yield break;
                }
                if(currentState != EnemyStates.SearchingAlert)
                {
                    StopCoroutine(IsSearchingAlert);
                    IsSearchingAlert = null;
                    yield break;
                }
                else
                {

                }
                yield return null;
            }
        }
    }
    //Searching target while alert script ends-----------------------


    public virtual void FR_WanderingAround(Vector3 hitpoint)
    {
        var minDist = 10f;
        var point = hitpoint;
        var distance = Vector3.Distance(point, transform.position);
        if (distance < minDist) return;
        if (WanderingAround != null) return;
        WanderingAround = StartCoroutine(StartWanderingAround(point));

    }
    protected Coroutine WanderingAround;
    protected IEnumerator StartWanderingAround(Vector3 point)
    {
        if (currentState != EnemyStates.FR_Wandering) { WanderingAround = null; yield break; }
        while (currentState == EnemyStates.FR_Wandering)
        {
            var distance = Vector3.Distance(point, transform.position);
            WalkToPos(point);
            destinasion = agent.destination;
            transform.LookAt(agent.velocity + transform.position);
            if(distance < agent.stoppingDistance)
            {
                currentState = EnemyStates.FR_LookAround;
                WanderingAround = null;
                StopMove();
                yield break;
            }
            if(currentState != EnemyStates.FR_Wandering) { WanderingAround = null; yield break; }
            yield return null;
        }
    }


    protected int indexPatrol = 0;
    public virtual void Patrol()
    {
        if(patrolRoutes.Length > 1)
        {
            WalkToPos(patrolRoutes[indexPatrol]);
            destinasion = agent.destination;
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

    public virtual void Suspicious()
    {
        if(IsSuspicious == null)
        {
            IsSuspicious = StartSuspicious();
            StartCoroutine(IsSuspicious);
        }
    }
    protected IEnumerator IsSuspicious;
    public virtual IEnumerator StartSuspicious()
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
                
                
                    transform.rotation = Quaternion.Slerp(transform.rotation, lookDir, lookSpeed * Time.deltaTime);
                    delayTime += Time.deltaTime;
                    yield return 0;
                
            }
            currentState = EnemyStates.SuspiciousApproach;
        }
        IsSuspicious = null;
    }


    public virtual void SuspiciousApproach()
    {
        if(location == Vector3.zero)
        {
            currentState = defaultState;
        }
        else
        {
            WalkToPos(location);
            transform.LookAt(agent.velocity + transform.position);
            var distance = Vector3.Distance(location, transform.position);
            if (distance < 0.5f)
            {
                agent.velocity = Vector3.zero;
                StopMove();
                currentState = EnemyStates.SuspiciousLookAround;
            }
        }
    }

    public virtual void SuspiciousLookAround()
    {
        if(IsSuspiciousLooking == null)
        {
            IsSuspiciousLooking = StartSuspiciousLookAround();
            StartCoroutine(IsSuspiciousLooking);
        }
    }
    protected IEnumerator IsSuspiciousLooking;
    public virtual IEnumerator StartSuspiciousLookAround()
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


    public virtual void SuspiciousRunTowards()
    {
        if (location == Vector3.zero)
        {
            currentState = defaultState;
        }
        else
        {
            RunToPos(location);
            transform.LookAt(agent.velocity + transform.position);
            if (!agent.pathPending)
            {
                if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                {
                    SendInfoToAlertBar(0f);
                    StopMove();
                    currentState = EnemyStates.LookAroundSearching;
                }

            }
        }
    }


    public virtual void TrackTrails()
    {
        if(IsTrackingTrails == null)
        {
            IsTrackingTrails = StartTrackingTrails();
            StartCoroutine(IsTrackingTrails);
        }
    }
    protected IEnumerator IsTrackingTrails;
    public virtual IEnumerator StartTrackingTrails()
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
    protected int trailIndex = 0;
    public virtual void AddTrails(Vector3 newTrails)
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
    public virtual Vector3 GetLastSeenPosition()
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


    //Reset enemy when Level restarts
    public void ResetEnemy()
    {
        if(defaultState == EnemyStates.FR_Patrol)
        {
            indexPatrol = patrolRoutes.Length - 1;
            agent.Warp(patrolRoutes[indexPatrol]);
            rb.isKinematic = true;
            rb.isKinematic = false;
            target = null;
            currentState = defaultState;
        }
        else if(defaultState == EnemyStates.FR_Idle)
        {
            agent.Warp(idlePos);
            //transform.position = idlePos;
            transform.LookAt(idleLookRotation);
            rb.isKinematic = true;
            rb.isKinematic = false;
            target = null;
            currentState = defaultState;
        }
        else
        {
            target = null;
            currentState = defaultState;
        }
        baseEnemyAlertBar.ResetEnemyAlertBar();
    }
    //Reset enemy ends---------------------------------



    public event EventHandler<SendEventToAlertBarScrArgs> SendEventToAlertBarScr;
    public class SendEventToAlertBarScrArgs : EventArgs { public float adder; }
    public virtual void SendInfoToAlertBar(float adder)
    {
        SendEventToAlertBarScr?.Invoke(this, new SendEventToAlertBarScrArgs { adder = adder });
    }


    
    //Handles Alert bar communications.
    public virtual void AlertBarIsEmptyReceiver(object sender, EventArgs e)
    {
        AlertBarIsEmpty();
    }
    public virtual void AlertBarIsEmpty()
    {
        lastCharToHitMe = null;
        currentState = defaultState;
        target = null;
        RemoveEnemyFromAlertList();
    }
    //Alert bar communication script ends-----------------------------


    public virtual void SoundToLogicReceiver(object sender, BaseEnemySoundController.SoundToLogicArgs e)
    {
        ReceivingAudioInfo(e.alertLevel, e.location);
    }
    protected Vector3 location;
    public virtual void ReceivingAudioInfo(float alertLevel, Vector3 target)
    {
        var lowAlertLevel = 1f;
        var highAlertLevel = 2f;
        if (currentState == EnemyStates.ChaseTarget || currentState == EnemyStates.Attack || currentState == EnemyStates.ChaseLastKnownPosition || currentState == EnemyStates.Null || currentState == EnemyStates.SuspiciousRunTowards)
        {

        }
        else
        {
            if(alertLevel == lowAlertLevel)
            {
                currentState = EnemyStates.Suspicious;
                location = new Vector3(target.x, transform.position.y, target.z);
            }
            else if(alertLevel == highAlertLevel)
            {
                SendInfoToAlertBar(30f);
                AddEnemyToAlertList();
                currentState = EnemyStates.SuspiciousRunTowards;
                location = new Vector3(target.x, transform.position.y, target.z);
            }
        }
    }


    private void SendDmgToLogicReceiver(object sender, EntityHealthController.SendDmgToLogicArgs e)
    {
        CheckStatus(e.currentHealth);
        KnockBack(e.dmgSender, e.knckBckPwr);
    }



    private void CheckStatus(float health)
    {
        if(health <= 0)
        {
            PlaySpecialActor(null);
        }
    }

    public void PlaySpecialActor(RuntimeAnimatorController overrideController)
    {
        var animator = specialActor.TryGetComponent(out Animator anim) ? anim : null;
        if (animator == null) return;
        if (overrideController == overrideSAController && overrideSAController != null && specialActor.activeSelf == true) return;
        if (overrideController == null && overrideSAController == null && specialActor.activeSelf == true) return;
        overrideSAController = overrideController;
        specialActor.transform.position = transform.position;
        if (overrideSAController == null)
        {
            animator.runtimeAnimatorController = defaultSAController;
            specialActor.SetActive(true);
            visuals.SetActive(false);
        }
        else
        {
            animator.runtimeAnimatorController = overrideSAController;
            specialActor.SetActive(true);
            visuals.SetActive(false);
        }
    }

    public void DisableSpecialActor()
    {
        visuals.SetActive(true);
        specialActor.SetActive(false);
        specialActor.transform.position = transform.position;
    }

    IEnumerator IsKBCooldown;
    public IEnumerator KBCoolDown()
    {
        var cooldownTime = 0f;
        var cooldownRate = 0.5f;
        while (cooldownTime < cooldownRate)
        {
            StopMove();
            cooldownTime += Time.deltaTime;
            yield return 0;
        }
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = true;
        IsKBCooldown = null;
    }

    public void KnockBack(Transform sender, float power)
    {
        if(IsKBCooldown == null && gameObject.activeSelf == true)
        {
            rb.isKinematic = false;
            IsKBCooldown = KBCoolDown();
            StartCoroutine(IsKBCooldown);
            agent.velocity = Vector3.zero;
            agent.isStopped = false;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            var direction = (transform.position - sender.position).normalized;
            rb.AddForce(direction * power, ForceMode.Impulse);
        }
    }


    public FactionBaseSO.EnemyFactions GetCurrentFaction()
    {
        return currentFaction;
    }


    public event EventHandler PlayIdleAudioOnUpdateEvent;
    Coroutine DelayAudioPlay;
    IEnumerator StartDelayAudio(int randomSeconds)
    {
        var timeCount = 0f;
        var timeRate = (float)randomSeconds;
        while(timeCount < timeRate)
        {
            timeCount += Time.deltaTime;
            yield return null;
        }
        DelayAudioPlay = null;
        PlayIdleAudioOnUpdateEvent?.Invoke(this, EventArgs.Empty);
    }
    private void PlayAudioOnUpdate()
    {
        if (DelayAudioPlay != null) return;
        DelayAudioPlay = StartCoroutine(StartDelayAudio(Random.Range(10, 15)));
    }

    private void AddEnemyToAlertList()
    {
        ChaseMusicManager.instance.AddEnemyAlert(gameObject);
    }

    private void RemoveEnemyFromAlertList()
    {
        ChaseMusicManager.instance.RemoveEnemyAlert(gameObject);
    }

    private void Update()
    {
        StateController();
        CalculateCenterBody();
        PlayAudioOnUpdate();
        //EnemyAnimEvent?.Invoke(this, new EnemyAnimEventArgs { currentState = currentState, currentMoveState = currentMoveState });
    }


    private void OnCollisionEnter(Collision collision)
    {
        var direction = (collision.transform.position - transform.position).normalized;
        if (collision.transform.TryGetComponent(out PlayerLogic plr))
        {
            var dot = Vector3.Dot(transform.forward, direction);
            if(dot > 0.1)
            {
                target = collision.transform;
                currentState = EnemyStates.ChaseTarget;
            }
        }
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
