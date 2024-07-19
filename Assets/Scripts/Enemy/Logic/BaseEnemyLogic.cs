using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEnemyLogic : MonoBehaviour
{
    [SerializeField] BaseSight baseSight;

    [SerializeField] Vector3[] targetTrails;

    private void Start()
    {
        baseSight.SendTarget += SendTargetReceiver;
    }

    private void SendTargetReceiver(object sender, BaseSight.SendTargetInfo e)
    {
        LookAtPlayer(e.target);
    }

    private void LookAtPlayer(Transform target)
    {
        transform.LookAt(new Vector3(target.position.x, transform.position.y, target.position.z));
    }

    int trailIndex = 0;
    private void AddTrails(Vector3 newTrails)
    {
        targetTrails[trailIndex] = newTrails;
        if(trailIndex < targetTrails.Length)
        {
            trailIndex++;
        }
        else if(trailIndex == targetTrails.Length)
        {
            trailIndex = 0;
        }
    }
}
