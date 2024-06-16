using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestEnemy : MonoBehaviour
{
    [SerializeField] Material m_Material;
    [SerializeField] Rigidbody rb;
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
}
