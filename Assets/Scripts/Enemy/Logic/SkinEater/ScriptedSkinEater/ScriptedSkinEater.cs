using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class ScriptedSkinEater : BaseEnemyLogic
{
    [Header("LayerMask")]
    [SerializeField] LayerMask characters;

    

    private void OnEnable()
    {
        currentState = defaultState;
        SniffCount();
        //SniffTargetLocation();
    }

    private void OnDisable()
    {
        IsLookingAroundSearching = null;
    }

    public override void LookAroundSearching()
    {
        lastCharToHitMe = null;
        if (IsLookingAroundSearching == null)
        {
            IsLookingAroundSearching = LookingAroundAndSearch();
            StartCoroutine(IsLookingAroundSearching);
        }
        else
        {

        }
    }


    private void SniffCount()
    {
        if(SniffCounting == null)
        {
            SniffCounting = StartSniffCount();
            StartCoroutine(SniffCounting);
        }
    }
    IEnumerator SniffCounting;
    IEnumerator StartSniffCount()
    {
        var coolTime = 0f;
        var coolRate = 30f;
        while(transform.gameObject.activeSelf == true)
        {
            coolTime = 0f;
            while(coolTime < coolRate)
            {
                Debug.Log(coolTime);
                coolTime += Time.deltaTime;
                yield return null;
            }
            if(transform.gameObject.activeSelf == false)
            {
                StopCoroutine(SniffCounting);
                SniffCounting = null;
                break;
            }
            else
            {
                SniffTargetLocation();
            } 
        }
    }
    private void SniffTargetLocation()
    {
        var radius = 100f;
        var sphere = RotaryHeart.Lib.PhysicsExtension.Physics.OverlapSphere(transform.position, radius, characters, RotaryHeart.Lib.PhysicsExtension.PreviewCondition.None);

        for (int i = 0; i < sphere.Length; i++)
        {
            if (sphere[i].TryGetComponent(out EntityHealthController trgt))
            {
                if(currentState == EnemyStates.ChaseTarget || currentState == EnemyStates.ChaseLastKnownPosition || currentState == EnemyStates.SuspiciousRunTowards)
                {

                }
                else if (sphere[i].transform != transform)
                {
                    target = sphere[i].transform;
                    currentState = EnemyStates.ChaseTarget;
                }
            }
        }
    }
}
