using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class EnemyScriptBase : MonoBehaviour, IInflictDamage, IMakeSound
{
    [Header("Basic Variables")]
    [SerializeField] float enemyHealth;
    [SerializeField] Material m_Material;
    [SerializeField] Rigidbody rb;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Transform targetPos;
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
        Wandering,
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

    [Header("Hearing Settings")]
    [SerializeField] float hearingMultiplier;


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
            Vector3 direction = (transform.position - sender.position).normalized;
            rb.AddForce(direction * knockBackPwr, ForceMode.Impulse);
        }
    }

    //Function that controls Enemy states
    private void StateController()
    {
        if (state == EnemyState.Chasing)
        {
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
        if(state == EnemyState.LookAroundChase)
        {
            LookAroundChase();
        }
        if(state == EnemyState.Wandering)
        {
            Wandering();
        }
        if(state == EnemyState.ChasingLastKnown)
        {
            ChaseLastKnownPos(GetLastSeenPos(targetPlayer));
        }
        if(state == EnemyState.Suspicious)
        {
            Suspicious();
        }
        if(state == EnemyState.SuspiciousLook)
        {
            //Code is handled by SoundReceiver
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
            float distance = Vector3.Distance(plrPos.position, transform.position);
            if (distance < stopDistance)
            {
                atkScr.MainAttack(plrPos, transform);
                agent.isStopped = true;
                agent.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
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
        float lookRate = 0;
        float rate = 2.5f;
        int looking = 0;
        int lookMax = 3;
        while (looking < lookMax)
        {
            Vector3 direction = new Vector3(transform.position.x + Random.Range(-1f, 1f), transform.position.y, transform.position.z + Random.Range(-1f, 1f));
            Quaternion lookDir = Quaternion.LookRotation(transform.position - direction);
            while (lookRate < rate)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, lookDir, 10f * Time.deltaTime);
                lookRate += Time.deltaTime;
                yield return 0;
            }
            lookRate = 0;
            looking++;
        }
        IsLookingAround = null;
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
        int lookAmounts = 4;
        float lookRate = 1.3f;
        if (lookRate < delayLookChase && looking < lookAmounts && lerpRotateChaseRunning == null && HitCooldown == null)
        {
            Vector3 direction = new Vector3(transform.position.x + Random.Range(-1f, 1f), transform.position.y, transform.position.z + Random.Range(-1f, 1f));
            Quaternion lookDir = Quaternion.LookRotation(transform.position - direction);
            lerpRotateChaseRunning = lerpRotateChasing(lookDir);
            StartCoroutine(lerpRotateChaseRunning);
        }
        if (looking >= lookAmounts && lerpRotateChaseRunning == null && HitCooldown == null)
        {
            state = EnemyState.Wandering;
            looking = 0;
            delayLookChase = 0;
        }
        if(lookRate > delayLookChase && looking < lookAmounts && lerpRotateChaseRunning == null && HitCooldown == null)
        {
            delayLookChase += Time.deltaTime;
        }
    }
    IEnumerator lerpRotateChaseRunning = null;
    float IE_moveTime = 0;
    IEnumerator lerpRotateChasing(Quaternion lookDir)
    {
        float maxTime = 1f;
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
    int indexPatrol = 0;
    private void Patrolling()
    {
        agent.enabled = true;
        agent.speed = defaultSpeed;
        agent.acceleration = defaultAcc;
        agent.isStopped = false;
        agent.destination = patrolRoute[indexPatrol];
        transform.LookAt(agent.velocity + transform.position);
        float distance = Vector3.Distance(transform.position, agent.destination);
        if (distance <= 1)
        {
            indexPatrol++;
        }
        if (indexPatrol == patrolRoute.Length)
        {
            indexPatrol = 0;
        }
    }


    //Function that controls Enemy wandering
    private Vector3 direction;
    private Vector3 newDir;
    IEnumerator IsWandering = null;
    float IE_wanderTime = 0;
    IEnumerator WanderToPos(float value1, float value2)
    {
        float wanderRate = 3f;
        while (IE_wanderTime < wanderRate && state == EnemyState.Wandering && IsWandering != null && HitCooldown == null)
        {
            direction = new Vector3(value1, 0, value2);
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
        float nearestDistance = 15f;
        int maxCount = plrTrails.Length;
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
            float distance = Vector3.Distance(lastKnownPos, transform.position);
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
    float IE_lookTime = 0;
    IEnumerator SuspiciousLooking()
    {
        float rate = 0.5f;
        Vector3 direction = new Vector3(targetInvestigation.x - transform.position.x, transform.position.y, targetInvestigation.z - transform.position.z);
        Quaternion lookDir = Quaternion.LookRotation(transform.position - direction);
        while (IE_lookTime < rate)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, lookDir, 10f * Time.deltaTime);
            IE_lookTime += Time.deltaTime;
            yield return 0;
        }
        IE_lookTime = 0;
        IsSusLooking = null;
        state = EnemyState.Suspicious;
    }
    private void SuspiciousLook()
    {
        if(IsSusLooking == null)
        {
            IsSusLooking = SuspiciousLooking();
            StartCoroutine(IsSusLooking);
        }
    }

    IEnumerator IsSuspicious;
    IEnumerator SuspiciousTime()
    {
        float IE_time = 0;
        float rate = 2f;
        while(IE_time < rate)
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
        float distance = Vector3.Distance(targetInvestigation, transform.position);
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
        float distance = Vector3.Distance(targetInvestigation, transform.position);
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
            float distance = Vector3.Distance(soundSrc, transform.position);
            if (distance < (soundDist * hearingMultiplier))
            {
                targetInvestigation = soundSrc;
                state = EnemyState.SuspiciousLook;
                SuspiciousLook();

            }
            if (distance < ((soundDist * hearingMultiplier) / 2))
            {
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
