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
    [SerializeField] float defaultRayDist;
    [SerializeField] float defaultMaxVision;
    [SerializeField] float minDotProduct;
    [SerializeField] float seenMaxVision;
    [SerializeField] float currentMaxVision;
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
        defaultRayDist = baseSight.maxRayDist;
        defaultMaxVision = baseSight.maxVision;
        seenMaxVision = baseSight.maxVision * 4;
        minDotProduct = baseSight.minDotProduct;
        currentMaxVision = defaultMaxVision;
    }

    private void EnemySight()
    {
        seeCharacters = RotaryHeart.Lib.PhysicsExtension.Physics.BoxCastAll(transform.position + Vector3.up * 1f, new Vector3(10f, 0.5f, 0.05f), transform.forward, Quaternion.LookRotation(transform.forward), defaultRayDist, characters);

        for (int i = 0; i < seeCharacters.Length; i++)
        {
            CheckDistance(seeCharacters[i]);
        }
        
    }


    //Checks distance then if its friend or enemy and events related.
    private void CheckDistance(RaycastHit hitInfo)
    {
        if(hitInfo.distance < currentMaxVision)
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
        bool canSee = RotaryHeart.Lib.PhysicsExtension.Physics.Raycast(transform.position, direction, out RaycastHit hit, RotaryHeart.Lib.PhysicsExtension.PreviewCondition.None);
        var distance = Vector3.Distance(hitInfo.transform.position, transform.position);
        if(hitInfo.transform.TryGetComponent(out PlayerLogic plr))
        {
            if (hit.transform == hitInfo.transform && distance <= currentMaxVision)
            {
                StopAllCoroutines();
                currentMaxVision = seenMaxVision;
                Debug.Log("Player Seen!");
                currentState = SightStates.HasSeen;
                targetLook = hitInfo.transform;
                SendTarget?.Invoke(this, new SendTargetInfo { target = hitInfo.transform });
                
            }
            else if (hit.transform != hitInfo.transform && currentState == SightStates.HasSeen)
            {
                
                Debug.Log("Player Lost!");
                currentState = SightStates.HasNotSeen;
                SendTarget?.Invoke(this, new SendTargetInfo { target = null });
                IsTrackingTracks = TrackTargetTracks();
                IsCountingDown = CountdownToLoseTarget();
                StartCoroutine(IsTrackingTracks);
                StartCoroutine(IsCountingDown);
            }
        }
        else if(hitInfo.transform.TryGetComponent(out BaseSight friend))
        {
            if(hitInfo.transform == transform )
            {

            }
            else
            {
                if(hit.transform == hitInfo.transform && distance <= currentMaxVision)
                {
                    Debug.Log(hitInfo.transform.name);
                    Debug.Log("Is Fren!");
                }
            }
        }
        else
        {
            
        }
    }
    // Friend or enemy check ends------------------------



    //Handles sending target trails and events related.
    public event EventHandler<SendTargetPosArgs> SendTargetPos;
    public class SendTargetPosArgs : EventArgs { public Vector3 target; }
    IEnumerator IsTrackingTracks;
    IEnumerator TrackTargetTracks()
    {
        var trackTime = 0f;
        var trackRate = 0.1f;
        while(targetLook != null)
        {
            while(trackTime < trackRate)
            {
                trackTime += Time.deltaTime * 10;
                yield return null;
            }
            trackTime = 0f;
            if(targetLook == null)
            {

            }
            else
            {
                SendTargetPos?.Invoke(this, new SendTargetPosArgs { target = targetLook.position });
            }
        }
        IsTrackingTracks = null;
    }
    // Target tracking script ends-------------------------------------------



    //Handles counting down before stop tracking trails.
    IEnumerator IsCountingDown;
    IEnumerator CountdownToLoseTarget()
    {
        var loseTime = 0f;
        var loseRate = 1f;
        while(loseTime < loseRate)
        {
            loseTime += Time.deltaTime;
            yield return 0;
        }
        if(IsTrackingTracks != null)
        {
            StopCoroutine(IsTrackingTracks);
            IsTrackingTracks = null;
            targetLook = null;
        }
        else
        {
            targetLook = null;
        }
        IsCountingDown = null;
    }
    //Counting down to losing trails script ends--------------------------------




    private void Update()
    {
        EnemySight();
    }
}
