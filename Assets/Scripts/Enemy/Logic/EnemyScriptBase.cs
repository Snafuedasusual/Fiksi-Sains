using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyScriptBase : MonoBehaviour
{
    [SerializeField] Material m_Material;
    [SerializeField] Rigidbody rb;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Transform targetPos;
    [SerializeField] float stopDistance;

    public void InflictDamage(Transform sender, float knockBackPwr, float damage)
    {
        if(HitCooldown == null)
        {
            m_Material.color = Color.green;
            KnockBack(sender, knockBackPwr);
            StartCoroutine(Cooldown());
        }
        
    }

    IEnumerator HitCooldown;

    private IEnumerator Cooldown()
    {
        HitCooldown = Cooldown();
        yield return new WaitForSecondsRealtime(1.5f);
        m_Material.color = Color.red;
        HitCooldown = null;
    }
    
    private void KnockBack(Transform sender, float knockBackPwr)
    {
        Vector3 direction = (transform.position - sender.position).normalized;
        rb.AddForce(direction * knockBackPwr, ForceMode.Impulse);
    }


    private void Start()
    {
         m_Material.color = Color.red;
    }

    public void ChasePlayer(Transform plrPos)
    {
        agent.destination = plrPos.position;
        agent.speed = 3.5f;
        agent.acceleration = 50f;
        float distance = Vector3.Distance(plrPos.position, transform.position);
        if(distance < stopDistance)
        {
            agent.isStopped = true;
        }
        else
        {
            agent.isStopped = false;
            agent.destination = plrPos.position;
        }
    }
}
