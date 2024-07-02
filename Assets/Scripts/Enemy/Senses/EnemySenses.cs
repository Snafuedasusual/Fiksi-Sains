using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemySenses : MonoBehaviour
{
    [SerializeField] private EnemyScriptBase enemyScript;
    [SerializeField] private LayerMask player;
    [SerializeField] private float maxVision;
    [SerializeField] private float maxRayDist;
    [SerializeField] private float minDotProduct;
    RaycastHit hitInfo;
    RaycastHit secondCheck;

    private bool knowsTarget = false;
    Transform targetSpotted;

    private void EnemySight()
    {
        bool seePlayer = RotaryHeart.Lib.PhysicsExtension.Physics.BoxCast(transform.position + Vector3.up * 1f, new Vector3(10f, 0.5f, 0.05f), transform.forward, out hitInfo, Quaternion.LookRotation(transform.forward), maxRayDist, player);
        if (seePlayer)
        {
            Vector3 direction = hitInfo.transform.position - transform.position;
            float dotDir = Vector3.Dot(transform.forward, direction.normalized);
            if (dotDir >= minDotProduct)
            {
                if (RotaryHeart.Lib.PhysicsExtension.Physics.Raycast(transform.position, direction + Vector3.up * 0.01f, out secondCheck))
                {
                    
                    float distance = Vector3.Distance(secondCheck.transform.position, transform.position);
                    if(secondCheck.transform == hitInfo.transform && distance <= maxVision)
                    {
                        //Debug.Log("Player Seen!");
                        StopAllCoroutines();
                        IE_time = 0f;
                        enemyScript.state = EnemyScriptBase.EnemyState.Chasing;
                        enemyScript.targetPlayer = secondCheck.transform;
                        enemyScript.seenPlayer = secondCheck.transform;
                        knowsTarget = true;
                        targetSpotted = secondCheck.transform;
                        GetPlayerTrails();
                    }
                    else
                    {
                        if (knowsTarget == true && targetSpotted != null)
                        {
                            enemyScript.state = EnemyScriptBase.EnemyState.ChasingLastKnown;
                            knowsTarget = false;
                            enemyScript.seenPlayer = secondCheck.transform;
                            StartCoroutine(TimeBeforeLostPlayer(targetSpotted));
                        }
                        if(enemyScript.state == EnemyScriptBase.EnemyState.Patroling)
                        {
                            knowsTarget = false;
                        }
                        else
                        {
                            enemyScript.seenPlayer = null;

                        }
                    }
                }
            }
            else
            {
                
            }
        }
    }

    float IE_time = 0;
    IEnumerator TimeBeforeLostPlayer(Transform plr)
    {
        float timeRate = 1f;
        while (timeRate > IE_time)
        {
            if(timeRate > IE_time)
            {
                IE_time += Time.deltaTime;
            }
            else
            {

            }
            GetPlayerTrails();
            yield return 0;

        }
        targetSpotted = null;
        yield return null;
    }

    float IE_trailDelay;
    int IE_trailIndex;
    private void GetPlayerTrails()
    {
        if (targetSpotted != null)
        {
            float dropRate = 0.01f;
            if (IE_trailDelay < dropRate)
            {
                IE_trailDelay = IE_trailDelay + Time.deltaTime;
            }
            else
            {
                if (IE_trailIndex < enemyScript.plrTrails.Length)
                {
                    enemyScript.plrTrails[IE_trailIndex] = targetSpotted.transform.position;
                    IE_trailIndex++;
                }
                else
                {
                    IE_trailIndex = 0;
                    enemyScript.plrTrails[IE_trailIndex] = targetSpotted.transform.position;
                }
                IE_trailDelay = 0f;
            }
        }
    }


    private void Update()
    {
        EnemySight();
    }
}
