using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack_TerrorBird : Attack
{
    [Header("Attack Variables")]
    [SerializeField] float mainDmg;
    [SerializeField] float mainDmgKnckBck;
    public float mainDmgRange;
    [SerializeField] float secDmg;
    [SerializeField] float secDmgKnckBck;
    [SerializeField] float thirdDmg;
    [SerializeField] float thirdDmgKnckBck;
    [SerializeField] float fourthDmg;
    [SerializeField] float fourthDmgKnckBck;

    public override void MainAttack(Transform target, Transform sender)
    {
        if(AtkCooling == null)
        {
            if(Physics.Raycast(sender.position + Vector3.up * 0.75f, target.position - sender.position, out RaycastHit hit, mainDmgRange))
            {
                if(hit.transform == target.transform)
                {
                    if (hit.transform.TryGetComponent<IInflictDamage>(out IInflictDamage trgt))
                    {
                        trgt.DealDamage(mainDmg, sender, mainDmgKnckBck);
                        AtkCooling = AtkCooldown();
                        StartCoroutine(AtkCooling);
                    }
                    else
                    {

                    }
                }
            }
            
        }
        else
        {

        }
    }
    IEnumerator AtkCooling;
    float IE_atkCool = 0;
    IEnumerator AtkCooldown()
    {
        float rate = 0.3f;
        while (IE_atkCool < rate)
        {
            IE_atkCool += Time.deltaTime;
            yield return 0;
        }
        IE_atkCool = 0;
        AtkCooling = null;
    }
}
