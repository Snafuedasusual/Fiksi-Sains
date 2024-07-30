using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class EntityVisibilityController : MonoBehaviour
{
    [SerializeField] float visibilityBar;
    [SerializeField] Transform currentLightSrc;

    public void OnLightSourceHit(Transform lightSrc, float initialVisVal)
    {
        if (currentLightSrc != null)
        {
            if(lightSrc == currentLightSrc)
            {

            }
            else
            {
                currentLightSrc = CompareDistance(lightSrc, currentLightSrc);
                
            }
        }
        else if(currentLightSrc == null)
        {
            currentLightSrc = lightSrc;
        }
    }

    public void OnLightSourceLeave()
    {
        currentLightSrc = null;
        visibilityBar = 0;
    }

    void VisBarController(float value)
    {
        var
        visibilityBar = value;
    }

    private Transform CompareDistance(Transform source1, Transform source2)
    {
        var distance1 = Vector3.Distance(source1.position, transform.position);
        var distance2 = Vector3.Distance(source2.position, transform.position);

        if(distance1 < distance2)
        {
            return source1;
        }
        else if (distance1 > distance2)
        {
            return source2;
        }
        else
        {
            return currentLightSrc;
        }
    }
}
