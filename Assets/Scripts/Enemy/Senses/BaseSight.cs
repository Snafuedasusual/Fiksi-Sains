using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.Events;

public class BaseSight : MonoBehaviour
{
    [Header("Scriptable Objects")]
    [SerializeField] BaseSightSO baseSight;
    [Header("LayerMask")]
    [SerializeField] LayerMask characters;
    [Header("Variables")]
    [SerializeField] float maxRayDist;
    [SerializeField] float maxVision;
    [SerializeField] float minDotProduct;
    [SerializeField] RaycastHit[] seeCharacters;

    [Header("Enum States")]
    [SerializeField] private SightStates currentState;
    public enum SightStates
    {
        HasNotSeen,
        HasSeen,
    }

    private Transform targetLook;


    private void Start()
    {
        maxRayDist = baseSight.maxRayDist;
        maxVision = baseSight.maxVision;
        minDotProduct = baseSight.minDotProduct;
    }

    private void EnemySight()
    {
        seeCharacters = RotaryHeart.Lib.PhysicsExtension.Physics.BoxCastAll(transform.position + Vector3.up * 1f, new Vector3(10f, 0.5f, 0.05f), transform.forward, Quaternion.LookRotation(transform.forward), maxRayDist, characters);

        for (int i = 0; i < seeCharacters.Length; i++)
        {
            CheckDistance(seeCharacters[i]);
        }
        
    }


    //Checks distance then if its friend or enemy and events related.
    private void CheckDistance(RaycastHit hitInfo)
    {
        if(hitInfo.distance < maxVision)
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
        var direction = hitInfo.transform.position - transform.position;
        bool canSee = RotaryHeart.Lib.PhysicsExtension.Physics.Raycast(transform.position, direction, out RaycastHit hit, maxVision, RotaryHeart.Lib.PhysicsExtension.PreviewCondition.Both);
        if(hitInfo.transform.TryGetComponent(out PlayerLogic plr))
        {
            if (hit.transform == hitInfo.transform)
            {
                Debug.Log("Player Seen!");
                currentState = SightStates.HasSeen;
                targetLook = hitInfo.transform;
                SendTarget?.Invoke(this, new SendTargetInfo { target = hitInfo.transform });
            }
            else if (hit.transform != hitInfo.transform && currentState == SightStates.HasSeen)
            {
                Debug.Log("Player Lost!");
                currentState = SightStates.HasNotSeen;
                targetLook = null;
                SendTarget?.Invoke(this, new SendTargetInfo { target = hitInfo.transform });
            }
        }
        else if(hitInfo.transform.TryGetComponent(out BaseSight friend))
        {
            if(hitInfo.transform == transform)
            {

            }
            else
            {
                if(hit.transform == hitInfo.transform)
                {
                    Debug.Log(hitInfo.transform.name);
                    Debug.Log("Is Fren!");
                }
            }
        }
    }
    // Friend or enemy check ends------------------------


    public event EventHandler<SendTargetPosArgs> SendTargetPos;
    public class SendTargetPosArgs : EventArgs { public Vector3 target; }
    IEnumerator IsTrackingTracks;
    IEnumerator TrackTargetTracks()
    {
        var trackTime = 0f;
        var trackRate = 0.2f;
        while(targetLook != null)
        {
            while(trackTime < trackRate)
            {
                trackTime += Time.deltaTime;
                yield return null;
            }
            trackTime = 0f;
            SendTargetPos?.Invoke(this, new SendTargetPosArgs { target = targetLook.position });
            
        }
    }

    IEnumerator IsCountingDown;
    IEnumerator CountdownToLoseTarget()
    {
        var loseTime = 0f;
        var loseRate = 1.3f;
        while(loseTime < loseRate)
        {
            loseTime += Time.deltaTime;
            yield return 0;
        }
        if(IsTrackingTracks != null)
        {
            StopCoroutine(IsTrackingTracks);
            targetLook = null;
        }
        IsCountingDown = null;
    }
    private void Update()
    {
        EnemySight();
    }
}
