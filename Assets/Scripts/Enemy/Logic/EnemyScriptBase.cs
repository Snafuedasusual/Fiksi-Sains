using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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

    public enum EnemyState
    {
        Idle,
        Patroling,
        Searching,
        Chasing,
        ChasingLastKnown,
        LookAroundChase,
        LookAroundIdle
    }

    [SerializeField]public EnemyState state;
    [SerializeField]private EnemyState defaultState;

    public Transform targetPlayer;
    public Transform seenPlayer;

    public void InflictDamage(Transform sender, float knockBackPwr, float damage)
    {
        if(HitCooldown == null)
        {
            agent.isStopped = true;
            agent.velocity = Vector3.zero;
            m_Material.color = Color.green;
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
    }

    public void ChasePlayer(Transform plrPos)
    {
        if(plrPos != null)
        {
            agent.isStopped = false;
            agent.destination = plrPos.position;
            agent.speed = chaseSpeed;
            agent.acceleration = chaseAcc;
            transform.LookAt(plrPos);
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
        int lookAmounts = 3;
        float lookRate = 2f;
        if (lookRate < delayLookChase && looking < lookAmounts)
        {
            Vector3 direction = new Vector3(transform.position.x + Random.Range(-1f, 1f), transform.position.y, transform.position.z + Random.Range(-1f, 1f));
            Quaternion lookDir = Quaternion.LookRotation(transform.position - direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookDir, 1);
            delayLookChase = 0;
            looking++;
        }
        if (lookRate < delayLookChase && looking > lookAmounts)
        {
            state = EnemyState.Patroling;
            looking = 0;
            delayLookChase = 0;
        }
        if(lookRate > delayLookChase && looking < lookAmounts)
        {
            delayLookChase += Time.deltaTime;
        }
        else
        {
            state = EnemyState.Patroling;
            looking = 0;
            delayLookChase = 0;
        }
    }


    private void Idle()
    {
        //Do Nothing
    }

    int indexPatrol = 0;
    private void Patrolling()
    {
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
