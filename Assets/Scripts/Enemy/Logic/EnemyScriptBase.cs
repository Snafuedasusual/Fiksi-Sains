using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class EnemyScriptBase : MonoBehaviour, IHealthInterface, IMakeSounds
{
    [Header("Hearing Settings")]
    [SerializeField] float hearingMultiplier;

    [Header("AlertBar")]
    [SerializeField] AlertBarEnemy alertBar;

    [Header("Basic Variables")]
    [SerializeField] float enemyHealth;
    [SerializeField] Material m_Material;
    [SerializeField] Rigidbody rb;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] float stopDistance;
    [SerializeField] float atkRange;
    [SerializeField] Vector3[] patrolRoute;
    [SerializeField] float defaultSpeed;
    [SerializeField] float defaultAcc;
    [SerializeField] float chaseSpeed;
    [SerializeField] float chaseAcc;

    [Header("Enemy States and Others")]
    [SerializeField] public EnemyState state;
    [SerializeField] private EnemyState defaultState;
    public enum EnemyState
    {
        Idle,
        Patroling,
        ReturnToPatrol,
        WanderingIdle,
        Wandering,
        WanderingAlert,
        Chasing,
        ChasingLastKnown,
        LookAroundChase,
        LookAroundIdle,
        SuspiciousLook,
        Suspicious,
        SuspiciousApproach,
        SuspiciousApproachAlert,
        FollowFriend,
    }

    public Transform targetPlayer;
    public Vector3 targetInvestigation;
    public Vector3[] plrTrails = new Vector3[10];

    [Header("Interaction Scripts")]
    [SerializeField] Attack atkScr;
    


    public void DealDamage(float damage, Transform dmgSender, float knckBckPwr)
    {
        enemyHealth -= damage;
        transform.LookAt(dmgSender);
        if (enemyHealth < 0)
        {
            transform.gameObject.SetActive(false);
        }
        else
        {
            KnockBack(dmgSender, knckBckPwr);
        }

    }
    IEnumerator HitCooldown;
    private IEnumerator Cooldown()
    {
        HitCooldown = Cooldown();
        yield return new WaitForSecondsRealtime(0.5f);
        rb.isKinematic = true;
        rb.isKinematic = false;
        agent.enabled = true;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        agent.isStopped = false;
        m_Material.color = Color.red;
        HitCooldown = null;
    }
    public void KnockBack(Transform sender, float knockBackPwr)
    {
        if(HitCooldown == null)
        {
            StartCoroutine(Cooldown());
            agent.isStopped = true;
            agent.velocity = Vector3.zero;
            agent.enabled = false;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            m_Material.color = Color.green;
            var direction = (transform.position - sender.position).normalized;
            rb.AddForce(direction * knockBackPwr, ForceMode.Impulse);
        }
    }

    //Function that controls Enemy states
    private void StateController()
    {
        if (state == EnemyState.Chasing)
        {
            //alertBar.AddAlertness(50f);
            ChasePlayer(targetPlayer);
        }
        if (state == EnemyState.LookAroundIdle)
        {
            targetPlayer = null;
            LookAroundIdle();
        }
        if (state == EnemyState.Patroling)
        {
            targetPlayer = null;
            targetInvestigation = Vector3.zero;
            Patrolling();
        }
        if(state == EnemyState.ReturnToPatrol)
        {
            ReturnToPatrolRoute();
        }
        if(state == EnemyState.LookAroundChase)
        {
            //alertBar.AlertBarController();
            LookAroundChase();
        }
        if(state == EnemyState.WanderingAlert)
        {
            //alertBar.AlertBarController();
            Wandering();
        }
        if(state == EnemyState.ChasingLastKnown)
        {
            ChaseLastKnownPos(GetLastSeenPos(targetPlayer));
        }
        if(state == EnemyState.SuspiciousLook)
        {
            SuspiciousLook();
        }
        if(state == EnemyState.SuspiciousApproach)
        {
            SuspiciousApproach();
        }
        if(state == EnemyState.SuspiciousApproachAlert)
        {
            SuspiciousApproachAlert();
        }
    }


    //Function that controls Enemy chasing
    public void ChasePlayer(Transform plrPos)
    {
        if(plrPos != null && HitCooldown == null)
        {
            looking = 0;
            delayLookChase = 0;
            agent.isStopped = false;
            agent.destination = plrPos.position;
            agent.speed = chaseSpeed;
            agent.acceleration = chaseAcc;
            transform.LookAt(plrPos);
            var distance = Vector3.Distance(plrPos.position, transform.position);
            if (distance < stopDistance)
            {
                
                agent.isStopped = true;
                agent.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
            if(distance < 2)
            {

            }
            else
            {
                agent.isStopped = false;
                agent.destination = plrPos.position;
            }
        }
        else
        {

        }
    }



    IEnumerator IsLookingAround;
    IEnumerator LookingAround()
    {
        var lookRate = 0f;
        var rate = 2.5f;
        var looking = 0;
        var lookMax = 3;
        while (looking < lookMax)
        {
            var direction = new Vector3(transform.position.x + Random.Range(-1f, 1f), transform.position.y, transform.position.z + Random.Range(-1f, 1f));
            var lookDir = Quaternion.LookRotation(transform.position - direction);
            while (lookRate < rate && state == EnemyState.LookAroundIdle)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, lookDir, 10f * Time.deltaTime);
                lookRate += Time.deltaTime;
                if(state != EnemyState.LookAroundIdle)
                {
                    StopCoroutine(IsLookingAround);
                    IsLookingAround = null;
                }
                yield return 0;
            }
            lookRate = 0;
            looking++;
        }
        IsLookingAround = null;
        if(defaultState == EnemyState.Patroling)
        {
            state = EnemyState.ReturnToPatrol;
        }
        else
        {
            state = EnemyState.WanderingAlert;
        }
        looking = 0;
        lookRate = 0f;

    }
    private void LookAroundIdle()
    {
        if (IsLookingAround == null)
        {
            IsLookingAround = LookingAround();
            StartCoroutine(IsLookingAround);
        }
    }



    //Function that controls Enemy looking around on high alert
    float delayLookChase = 0f;
    int looking = 0;
    private void LookAroundChase()
    {
        var lookAmounts = 4;
        var lookRate = 1.3f;
        if (lookRate < delayLookChase && looking < lookAmounts && lerpRotateChaseRunning == null && HitCooldown == null)
        {
            var direction = new Vector3(transform.position.x + Random.Range(-1f, 1f), transform.position.y, transform.position.z + Random.Range(-1f, 1f));
            var lookDir = Quaternion.LookRotation(transform.position - direction);
            lerpRotateChaseRunning = lerpRotateChasing(lookDir);
            StartCoroutine(lerpRotateChaseRunning);
        }
        if (looking >= lookAmounts && lerpRotateChaseRunning == null && HitCooldown == null)
        {
            state = EnemyState.WanderingAlert;
            looking = 0;
            delayLookChase = 0;
        }
        if(lookRate > delayLookChase && looking < lookAmounts && lerpRotateChaseRunning == null && HitCooldown == null)
        {
            delayLookChase += Time.deltaTime;
        }
    }
    IEnumerator lerpRotateChaseRunning = null;
    IEnumerator lerpRotateChasing(Quaternion lookDir)
    {
        var IE_moveTime = 0f;
        var maxTime = 1f;
        while (IE_moveTime < maxTime && state == EnemyState.LookAroundChase && lerpRotateChaseRunning != null)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, lookDir, 10f * Time.deltaTime);
            IE_moveTime += Time.deltaTime;
            yield return 0;
        }
        lerpRotateChaseRunning = null;
        looking++;
        delayLookChase = 0;
        IE_moveTime = 0;
        yield return null;
    }


    private void Idle()
    {
        //Do Nothing
    }


    //Function that controls Enemy patrolling
    [SerializeField] int indexPatrol = 0;
    private void Patrolling()
    {
        var direction = (patrolRoute[indexPatrol] - rb.position).normalized;
        rb.MovePosition(rb.position + direction * (defaultSpeed) * Time.deltaTime);
        transform.LookAt(new Vector3(patrolRoute[indexPatrol].x, transform.position.y, patrolRoute[indexPatrol].z));
        var distance = Vector3.Distance(patrolRoute[indexPatrol], transform.position);
        if (distance <= 0.5f)
        {
            indexPatrol++;
        }
        if (indexPatrol == patrolRoute.Length)
        {
            indexPatrol = 0;
        }
    }

    private void ReturnToPatrolRoute()
    {
        agent.enabled = true;
        agent.speed = defaultSpeed;
        agent.acceleration = defaultAcc;
        agent.isStopped = false;
        agent.destination = patrolRoute[indexPatrol];
        float distance = Vector3.Distance(patrolRoute[indexPatrol], transform.position);
        if (distance < 0.5)
        {
            state = EnemyState.Patroling;
            agent.isStopped = true;
            agent.velocity = Vector3.zero;
        }

    }


    //Function that controls Enemy wandering
    IEnumerator IsWandering = null;
    float IE_wanderTime = 0;
    IEnumerator WanderToPos(float value1, float value2)
    {
        var wanderRate = 3f;
        while (IE_wanderTime < wanderRate && state == EnemyState.WanderingAlert && IsWandering != null && HitCooldown == null)
        {
            var direction = new Vector3(value1, 0, value2);
            transform.LookAt(transform.position + direction);
            bool canMove = RotaryHeart.Lib.PhysicsExtension.Physics.Raycast(transform.position + Vector3.up * 0.5f, transform.forward, out RaycastHit hit, 2.5f, RotaryHeart.Lib.PhysicsExtension.PreviewCondition.Game);
            IE_wanderTime += Time.deltaTime;
            if (canMove)
            {
                StopCoroutine(IsWandering);
                IsWandering = null;
                Wandering();
            }
            else
            {
                rb.MovePosition(rb.position + direction * (defaultSpeed - 2.5f) * Time.deltaTime);
            }
            yield return 0;
        }
        state = EnemyState.LookAroundChase;
        IE_wanderTime = 0f;
        IsWandering = null;
        yield return null;
    }
    private void Wandering()
    {
        if(IsWandering == null && HitCooldown == null)
        { 
            agent.isStopped = true;
            agent.speed = defaultSpeed;
            agent.acceleration = defaultAcc;
            float val1 = Random.Range(-5, 5);
            float val2 = Random.Range(-5, 5);
            IsWandering = WanderToPos(val1, val2);
            StartCoroutine(IsWandering);

        }
        else
        {

        }
    }


    //Function that controls Enemy chasing last seen position
    int countingTrails = 0;
    public Vector3 lastSeenPos;
    private Vector3 GetLastSeenPos(Transform targetPlr)
    {
        var nearestDistance = 15f;
        var maxCount = plrTrails.Length;
        for (int i = 0; i < plrTrails.Length; i++)
        {
            float distance = Vector3.Distance(plrTrails[i], targetPlr.position);
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                ChaseLastKnownPos(plrTrails[i]);
                lastSeenPos = plrTrails[i];
            }
            else
            {
                countingTrails++;
            }
        }
        return lastSeenPos;

    }
    private void ChaseLastKnownPos(Vector3 lastKnownPos)
    {
        if(HitCooldown == null)
        {
            agent.destination = lastKnownPos;
            transform.LookAt(agent.velocity + transform.position);
            var distance = Vector3.Distance(lastKnownPos, transform.position);
            if (distance < 1)
            {
                state = EnemyState.LookAroundChase;
                targetPlayer = null;
            }
            else
            {

            }
        }
    }


    IEnumerator IsSusLooking;
    IEnumerator SuspiciousLooking()
    {
        var IE_lookTime = 0f;
        var rate = 2f;
        var direction = new Vector3(targetInvestigation.x - transform.position.x, transform.position.y, targetInvestigation.z - transform.position.z);
        var lookDir = Quaternion.LookRotation(transform.position - direction);
        while (IE_lookTime < rate && state == EnemyState.SuspiciousLook)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, lookDir, 10f * Time.deltaTime);
            IE_lookTime += Time.deltaTime;
            yield return 0;
        }
        IE_lookTime = 0f;
        IsSusLooking = null;
        state = EnemyState.SuspiciousApproach;
    }
    private void SuspiciousLook()
    {
        if(IsSusLooking == null)
        {
            Debug.Log("Played!");
            IsSusLooking = SuspiciousLooking();
            StartCoroutine(IsSusLooking);
        }
    }

    IEnumerator IsSuspicious;
    IEnumerator SuspiciousTime()
    {
        var IE_time = 0f;
        var rate = 2f;
        while(IE_time < rate && state == EnemyState.Suspicious)
        {
            IE_time += Time.deltaTime;
            yield return 0;
        }
        IE_time = 0;
        state = EnemyState.SuspiciousApproach;
        IsSuspicious = null;
    }
    private void Suspicious()
    {
        if (IsSuspicious == null)
        {
            IsSuspicious = SuspiciousTime();
            StartCoroutine(IsSuspicious);
        }
        else
        {

        }
    }


    private void SuspiciousApproach()
    {
        agent.enabled = true;
        agent.speed = defaultSpeed;
        agent.acceleration = defaultAcc;
        agent.isStopped = false;
        agent.destination = targetInvestigation;
        transform.LookAt(agent.velocity + transform.position);
        var distance = Vector3.Distance(targetInvestigation, transform.position);
        if(distance < 0.5)
        {
            agent.isStopped = true;
            agent.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            state = EnemyState.LookAroundIdle;
        }
    }

    private void SuspiciousApproachAlert()
    {
        agent.enabled = true;
        agent.speed = chaseSpeed;
        agent.acceleration = chaseAcc;
        agent.isStopped = false;
        agent.destination = targetInvestigation;
        transform.LookAt(agent.velocity + transform.position);
        var distance = Vector3.Distance(targetInvestigation, transform.position);
        if (distance < 0.5)
        {
            agent.isStopped = true;
            agent.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            state = EnemyState.LookAroundChase;
        }
    }


    public void SoundReceiver(Vector3 soundSrc, float soundDist)
    {
        if(targetPlayer == null)
        {
            var distance = Vector3.Distance(soundSrc, transform.position);
            if (distance < (soundDist * hearingMultiplier))
            {
                //alertBar.AddAlertness(20f);
                targetInvestigation = soundSrc;
                state = EnemyState.SuspiciousLook;

            }
            if (distance < ((soundDist * hearingMultiplier) / 2))
            {
                //alertBar.AddAlertness(50f);
                targetInvestigation = soundSrc;
                state = EnemyState.SuspiciousApproachAlert;
            }
            else
            {

            }
        }
    }

    private void Update()
    {
        StateController();

    }

    private void Start()
    {
        m_Material.color = Color.red;
        state = defaultState;
    }
}
