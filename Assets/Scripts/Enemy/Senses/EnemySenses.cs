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
                        enemyScript.state = EnemyScriptBase.EnemyState.Chasing;
                        enemyScript.seenPlayer = secondCheck.transform;
                        knowsTarget = true;
                    }
                    else
                    {
                        if(knowsTarget == true)
                        {
                            enemyScript.state = EnemyScriptBase.EnemyState.LookAroundChase;
                            enemyScript.seenPlayer = null;
                            Debug.Log("Player Lost!");
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

    private void Update()
    {
        EnemySight();
    }
}
