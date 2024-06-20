using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySenses : MonoBehaviour
{
    [SerializeField] private LayerMask player;
    RaycastHit hitInfo;
    RaycastHit secondCheck;
    Vector3 directionChecker;
    private void EnemySight()
    {
        bool seePlayer = RotaryHeart.Lib.PhysicsExtension.Physics.BoxCast(transform.position + Vector3.up * 1f, new Vector3(10f, 0.5f, 0.05f), transform.forward, out hitInfo, Quaternion.LookRotation(transform.forward), 15f, player, RotaryHeart.Lib.PhysicsExtension.PreviewCondition.Both);
        if (seePlayer)
        {
            Vector3 direction = hitInfo.transform.position - transform.position;
            float dotDir = Vector3.Dot(transform.forward, direction.normalized);
            if (dotDir >= 0.425f)
            {
                if (RotaryHeart.Lib.PhysicsExtension.Physics.Raycast(transform.position, direction + Vector3.up * 0.01f, out secondCheck, RotaryHeart.Lib.PhysicsExtension.PreviewCondition.Both))
                {
                    if(secondCheck.transform == hitInfo.transform)
                    {
                        Debug.Log("Player Seen!");
                    }
                    else
                    {
                        Debug.Log("Player Lost!");
                    }
                }
            }
            else
            {
                
            }
        }
    }

    private void FixedUpdate()
    {
        EnemySight();
    }
}
