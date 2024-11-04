using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BaseSight : MonoBehaviour
{
    [Header("Scriptable Objects")]
    [SerializeField] BaseSightSO baseSight;
    [Header("LayerMask")]
    [SerializeField] LayerMask characters;
    [Header("Script References")]
    [SerializeField] BaseEnemyAlertBar baseAlertBar;
    [Header("Variables")]
    [SerializeField] float eyeHeight;
    [SerializeField] float defaultRayDist;
    [SerializeField] float defaultMaxVision;
    [SerializeField] float minDotProduct;
    [SerializeField] float alertMaxVision;
    [SerializeField] float currentMaxVision;
    [SerializeField] float minVisibility;
    [SerializeField] RaycastHit[] seeCharacters;

    [Header("Enum States")]
    [SerializeField] private SightStates currentState;
    public enum SightStates
    {
        HasNotSeen,
        HasSeen,
    }

    private Transform targetLook;

    private void InitializeScript()
    {
        eyeHeight = baseSight.eyeHeight;
        defaultRayDist = baseSight.maxRayDist;
        defaultMaxVision = baseSight.maxVision;
        alertMaxVision = baseSight.maxVision * 2;
        minDotProduct = baseSight.minDotProduct;
        currentMaxVision = defaultMaxVision;
        minVisibility = baseSight.minVisibilityBar;
    }

    private void Start()
    {
        InitializeScript();
        baseAlertBar.AlertBarIsEmpty += AlertBarIsEmptyReceiver;
        baseAlertBar.AlertBarIsNotEmpty += AlertBarIsNotEmptyReceiver;
    }

    private void AlertBarIsNotEmptyReceiver(object sender, EventArgs e)
    {
        OnHighAlert();
    }

    private void AlertBarIsEmptyReceiver(object sender, EventArgs e)
    {
        OnLowAlert();
    }

    private void OnHighAlert()
    {
        currentMaxVision = alertMaxVision;
    }
    private void OnLowAlert()
    {
        currentMaxVision = defaultMaxVision;
    }

    private void EnemySight()
    {
        seeCharacters = RotaryHeart.Lib.PhysicsExtension.Physics.BoxCastAll(transform.position + Vector3.up * eyeHeight, new Vector3(10f, 0.5f, 0.05f), transform.forward, Quaternion.LookRotation(transform.forward), defaultRayDist, characters);

        for (int i = 0; i < seeCharacters.Length; i++)
        {
            CheckDistance(seeCharacters[i]);
        }
        
    }


    //Checks distance then if its friend or enemy and events related.
    private void CheckDistance(RaycastHit hitInfo)
    {
        if(hitInfo.distance <= currentMaxVision)
        {
            var dotProduct = Vector3.Dot(transform.forward, (hitInfo.transform.position - transform.position).normalized);
            if(dotProduct > minDotProduct)
            {
                CheckFriendOrEnemy(hitInfo);
            }
        }
        else
        {

        }
    }
    public event EventHandler<SendTargetInfo> SendTarget;
    public class SendTargetInfo : EventArgs { public Transform target; }
    private void CheckFriendOrEnemy(RaycastHit hitInfo)
    {
        //var sightRange = defaultMaxVision;
        var direction = hitInfo.transform.position - transform.position;
        bool canSee = RotaryHeart.Lib.PhysicsExtension.Physics.Raycast(transform.position + Vector3.up * eyeHeight, direction, out RaycastHit hit, RotaryHeart.Lib.PhysicsExtension.PreviewCondition.None);
        var distance = Vector3.Distance(hitInfo.transform.position, transform.position);
        if (canSee)
        {
            if (hitInfo.transform.TryGetComponent(out PlayerLogic plr))
            {
                if (hit.transform == hitInfo.transform && distance <= currentMaxVision)
                {
                    if (hit.transform.GetComponent<EntityVisibilityController>().GetVisibilityBar() >= baseSight.minVisibilityBar)
                    {
                        StopAllCoroutines();
                        currentState = SightStates.HasSeen;
                        targetLook = hitInfo.transform;
                        SendTarget?.Invoke(this, new SendTargetInfo { target = hitInfo.transform });
                    }
                }
                else if (hit.transform != hitInfo.transform && currentState == SightStates.HasSeen)
                {
                    if(hit.transform.TryGetComponent(out BaseEnemyLogic entity))
                    {

                    }
                    else
                    {
                        currentState = SightStates.HasNotSeen;
                        SendTarget?.Invoke(this, new SendTargetInfo { target = null });
                    }
                }
            }
        }
    }
    // Friend or enemy check ends------------------------



    private void Update()
    {
        EnemySight();
    }
}
