using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class EnemyScriptBase : MonoBehaviour
{
    [SerializeField] Material m_Material;
    [SerializeField] Rigidbody rb;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Transform targetPos;
    [SerializeField] float stopDistance;
    [SerializeField] Vector3[] patrolRoute;
    [SerializeField] float defaultSpeed;
    [SerializeField] float defaultAcc;
    [SerializeField] float chaseSpeed;
    [SerializeField] float chaseAcc;
    [SerializeField] Transform cube;

    public enum EnemyState
    {
        Idle,
        Patroling,
        Wandering,
        Chasing,
        ChasingLastKnown,
        LookAroundChase,
        LookAroundIdle
    }

    [SerializeField]public EnemyState state;
    [SerializeField]private EnemyState defaultState;

    public Transform targetPlayer;
    public Transform seenPlayer;
    public Vector3[] plrTrails;

    public void InflictDamage(Transform sender, float knockBackPwr, float damage)
    {
        if(HitCooldown == null)
        {
            agent.isStopped = true;
            agent.velocity = Vector3.zero;
            m_Material.color = Color.green;
            transform.LookAt(sender);
            KnockBack(sender, knockBackPwr);
            StartCoroutine(Cooldown());
        }
        
    }

    IEnumerator HitCooldown;
    private IEnumerator Cooldown()
    {
        HitCooldown = Cooldown();
        yield return new WaitForSecondsRealtime(1f);
        agent.isStopped = false;
        m_Material.color = Color.red;
        HitCooldown = null;
    }
    private void KnockBack(Transform sender, float knockBackPwr)
    {
        Vector3 direction = (transform.position - sender.position).normalized;
        rb.AddForce(direction * knockBackPwr, ForceMode.Impulse);
    }

    private void StateController()
    {
        if (state == EnemyState.Chasing)
        {
            ChasePlayer(seenPlayer);
        }
        if (state == EnemyState.LookAroundIdle)
        {
            targetPlayer = null;
            LookAroundIdle();
        }
        if (state == EnemyState.Patroling)
        {
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
    }
    
    public void ChasePlayer(Transform plrPos)
    {
        if(plrPos != null)
        {
            looking = 0;
            delayLook = 0;
            delayLookChase = 0;
            agent.isStopped = false;
            agent.destination = plrPos.position;
            agent.speed = chaseSpeed;
            agent.acceleration = chaseAcc;
            transform.LookAt(plrPos);
            plrTrails = plrPos.GetComponent<PlayerLogic>().GetPlayerTrail();
            float distance = Vector3.Distance(plrPos.position, transform.position);
            if (distance < stopDistance)
            {
                agent.isStopped = true;
                agent.velocity = Vector3.zero;
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

    float delayLook = 0;
    private void LookAroundIdle()
    {
        agent.isStopped = true;
        float lookRate = 3.5f;
        if (lookRate < delayLook)
        {
            Vector3 direction = new Vector3(transform.position.x + Random.Range(-1f, 1f), transform.position.y, transform.position.z + Random.Range(-1f, 1f));
            Quaternion lookDir = Quaternion.LookRotation(transform.position - direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookDir, 1);
            delayLook = 0;
        }
        else
        {
            delayLook += Time.deltaTime;
        }
    }

    float delayLookChase = 0f;
    int looking = 0;
    private void LookAroundChase()
    {
        int lookAmounts = 4;
        float lookRate = 2f;
        if (lookRate < delayLookChase && looking < lookAmounts && lerpRotateChaseRunning == null)
        {
            Vector3 direction = new Vector3(transform.position.x + Random.Range(-1f, 1f), transform.position.y, transform.position.z + Random.Range(-1f, 1f));
            Quaternion lookDir = Quaternion.LookRotation(transform.position - direction);
            lerpRotateChaseRunning = lerpRotateChasing(lookDir);
            StartCoroutine(lerpRotateChaseRunning);
        }
        if (looking >= lookAmounts && lerpRotateChaseRunning == null)
        {
            state = EnemyState.Wandering;
            looking = 0;
            delayLookChase = 0;
        }
        if(lookRate > delayLookChase && looking < lookAmounts && lerpRotateChaseRunning == null)
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

    int indexPatrol = 0;
    private void Patrolling()
    {
        agent.enabled = true;
        agent.speed = defaultSpeed;
        agent.acceleration = defaultAcc;
        agent.isStopped = false;
        agent.destination = patrolRoute[indexPatrol];
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

    IEnumerator IsWandering = null;
    float IE_wanderTime = 0;
    IEnumerator WanderToPos(float value1, float value2)
    {
        float wanderRate = 3f;
        while (IE_wanderTime < wanderRate && state == EnemyState.Wandering && IsWandering != null)
        {
            Vector3 direction = new Vector3(value1, 0 , value2);
            transform.LookAt(transform.position + direction);
            bool canMove = RotaryHeart.Lib.PhysicsExtension.Physics.Raycast(transform.position + Vector3.up * 0.5f, transform.forward, out RaycastHit hit, 1.5f, RotaryHeart.Lib.PhysicsExtension.PreviewCondition.Game);
            rb.MovePosition(rb.position + direction * (defaultSpeed - 2.5f) * Time.deltaTime);
            IE_wanderTime += Time.deltaTime;
            if (canMove)
            {
                Vector3 newDir = new Vector3(Random.Range(-5f, 5f), 0, Random.Range(-5f, 5f));
                direction = newDir;
                /*
                IE_wanderTime = 0f;
                StopCoroutine(IsWandering);
                IsWandering = null;
                break;
                */
            }
            else
            {

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
        if(IsWandering == null)
        { 
            agent.isStopped = true;
            agent.speed = defaultSpeed;
            agent.acceleration = defaultAcc;
            float val1 = Random.Range(-5f, 5f);
            float val2 = Random.Range(-5f, 5f);
            IsWandering = WanderToPos(val1, val2);
            StartCoroutine(IsWandering);

        }
        else
        {

        }
    }

    int countingTrails = 0;
    Vector3 lastSeenPos;
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
        cube.transform.position = lastKnownPos;
        agent.destination = lastKnownPos;
        float distance = Vector3.Distance(lastKnownPos, transform.position);
        if(distance < 1)
        {
            state = EnemyState.LookAroundChase;
        }
        else
        {

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
