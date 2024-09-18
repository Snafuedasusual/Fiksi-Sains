using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack_TerrorBird : Attack, IInitializeScript
{
    [Header("Scriptable Objects")]
    [SerializeField] BaseEnemyStatsSO baseEnemyStats;


    [Header("Script References")]
    [SerializeField] BaseEnemyLogic baseEnemyLogic;


    [Header("Attack Variables")]
    [SerializeField] float mainDmg;
    [SerializeField] float mainDmgKnckBck;
    [SerializeField] float mainDmgRange;
    [SerializeField] float secDmg;
    [SerializeField] float secDmgKnckBck;
    [SerializeField] float secDmgRange;
    [SerializeField] float thirdDmg;
    [SerializeField] float thirdDmgKnckBck;
    [SerializeField] float thirdDmgRange;
    [SerializeField] float fourthDmg;
    [SerializeField] float fourthDmgKnckBck;
    [SerializeField] float fourthDmgRange;


    void InitializeVars()
    {
        mainDmg = baseEnemyStats.mainDmg;
        mainDmgKnckBck = baseEnemyStats.mainDmgKnckBck;
        mainDmgRange = baseEnemyStats.mainDmgRange;
        secDmg = baseEnemyStats.secDmg;
        secDmgKnckBck = baseEnemyStats.secDmgKnckBck;
        secDmgRange = baseEnemyStats.secDmgRange;
        thirdDmg = baseEnemyStats.thirdDmg;
        thirdDmgKnckBck = baseEnemyStats.thirdDmgKnckBck;
        thirdDmgRange = baseEnemyStats.thirdDmgRange;
        fourthDmg = baseEnemyStats.fourthDmg;
        fourthDmgKnckBck = baseEnemyStats.fourthDmgKnckBck;
        fourthDmgRange = baseEnemyStats.fourthDmgRange;
    }


    public void InitializeScript()
    {
        throw new System.NotImplementedException();
    }

    public void DeInitializeScript()
    {
        throw new System.NotImplementedException();
    }


    private void Start()
    {
        InitializeVars();
        baseEnemyLogic.OnAttackEvent += OnAttackEventReceiver;
    }



    private void OnAttackEventReceiver(object sender, BaseEnemyLogic.OnAttackEventArgs e)
    {
        MainAttack(e.target, e.sender);
    }

    public override void MainAttack(Transform target, Transform sender)
    {
        if(AtkCooling == null)
        {
            if(Physics.Raycast(sender.position + Vector3.up * 0.75f, target.position - sender.position, out RaycastHit hit, mainDmgRange))
            {
                AtkCooling = AtkCooldown();
                StartCoroutine(AtkCooling);
                if (hit.transform == target.transform)
                {
                    if (hit.transform.TryGetComponent(out EntityHealthController trgt))
                    {
                        trgt.DealDamage(mainDmg, sender, mainDmgKnckBck);
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
