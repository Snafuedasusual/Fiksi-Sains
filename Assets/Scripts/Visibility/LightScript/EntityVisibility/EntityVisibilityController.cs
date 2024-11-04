using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class EntityVisibilityController : MonoBehaviour
{
    [SerializeField] float visibilityBar;
    [SerializeField] Transform currentLightSrc;
    [SerializeField] float currentLightSrcRange;
    [SerializeField] float currentLightSrcPwr;


    public void OnLightSourceHit(Transform lightSrc, float power, float lightSrcRange)
    {
        if (currentLightSrc != null)
        {
            if(lightSrc == currentLightSrc)
            {

            }
            else
            {
                CompareDistance(lightSrc, currentLightSrc, currentLightSrcRange, lightSrcRange, currentLightSrcPwr, power);
                StartVisBarController();
            }
        }
        else if(currentLightSrc == null)
        {
            currentLightSrc = lightSrc;
            currentLightSrcRange = lightSrcRange;
            currentLightSrcPwr = power;
            StartVisBarController();
        }
    }




    public void OnLightSourceLeave()
    {
        currentLightSrc = null;
        currentLightSrcPwr = 0f;
        currentLightSrcRange = 0f;
        visibilityBar = 0;
        VisBarToUIFunc(visibilityBar);
    }




    private void StartVisBarController()
    {
        if(VisibilityBarController == null)
        {
            VisibilityBarController = VisBarController();
            StartCoroutine(VisibilityBarController);
        }
        else
        {

        }
    }
    IEnumerator VisibilityBarController;
    IEnumerator VisBarController()
    {
        
        while(currentLightSrc != null)
        {
            var rawDistance = (Vector3.Distance(currentLightSrc.position, transform.position));
            if(rawDistance > currentLightSrcRange * (1f + 0.3f))
            {
                OnLightSourceLeave();
                currentLightSrc = null; 
                break;
            }
            else
            {
                if(currentLightSrc == null)
                {
                    VisibilityBarController = null;
                    break;
                }
                else
                {
                    var distance = currentLightSrcRange - rawDistance;
                    if(distance > currentLightSrcRange * (1f + 0.05f))
                    {
                        visibilityBar = distance / currentLightSrcRange * 100;
                        if (visibilityBar > 100)
                        {
                            visibilityBar = 100;
                        }
                        else if(visibilityBar <= 0)
                        {
                            visibilityBar = 0;
                        }
                    }
                    else if(distance <= currentLightSrcRange * (1f - 0.05f))
                    {
                        visibilityBar = ((distance / currentLightSrcRange * 100) * currentLightSrcPwr);
                        if (visibilityBar > 100)
                        {
                            visibilityBar = 100;
                        }
                        else if (visibilityBar <= 0)
                        {
                            visibilityBar = 0;
                        }
                    }
                    VisBarToUIFunc(visibilityBar);
                }
            }
            yield return 0;
        }
        VisibilityBarController = null;
    }



    private void CompareDistance(Transform source1, Transform source2, float lightRange1, float lightRange2, float initialVisVal1, float initialVisVal2)
    {
        var distance1 = Vector3.Distance(source1.position, transform.position);
        var distance2 = Vector3.Distance(source2.position, transform.position);

        if (distance1 < distance2)
        {
            currentLightSrc = source1;
            currentLightSrcRange = lightRange1;
            currentLightSrcPwr = initialVisVal1;
        }
        else if (distance1 > distance2)
        {
            currentLightSrc = source2;
            currentLightSrcRange = lightRange2;
            currentLightSrcPwr = initialVisVal2;
        }
        else
        { 

        }
    }



    public float GetVisibilityBar()
    {
        return visibilityBar;
    }




    public event EventHandler<VisBarToUIArgs> VisBarToUI;
    public class VisBarToUIArgs : EventArgs { public float visibilityBarValue; }
    private void VisBarToUIFunc(float visBarValue)
    {
        VisBarToUI?.Invoke(this, new VisBarToUIArgs{visibilityBarValue = visBarValue});
    }
}
