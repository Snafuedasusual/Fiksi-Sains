using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AlertBarEnemy : MonoBehaviour
{
    [SerializeField] float alertBar = 0;
    [SerializeField] bool isCalled = false;
    public void AlertBarController()
    {
        StartCoroutine(BarIsDraining());
    }

    public void AddAlertness(float alertAdd)
    {
        isCalled = true;
        if(isCalled == true)
        {
            if (alertBar < 100)
            {
                alertBar += alertAdd;
            }
        }
        else
        {
            isCalled = false;
        }
        isCalled = false;
    }

    IEnumerator IsBarDraining;
    IEnumerator BarIsDraining()
    {
        float drainTime = 0;
        float drainRate = 1f;
        if (IsBarDraining != null)
        {

        }
        else
        {
            IsBarDraining = BarIsDraining();
            while(alertBar > 0)
            {
                if(isCalled == false)
                {
                    if(drainTime < drainRate)
                    {
                        drainTime += Time.deltaTime * 10;
                        yield return 0;
                    }
                    else
                    {
                        alertBar--;
                    }
                }
                else
                {
                    break;
                }
            }
        }
        IsBarDraining = null;
    }

    public float GetAlertBar()
    {
        return alertBar;
    }
}
