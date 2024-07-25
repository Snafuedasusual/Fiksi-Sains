using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBaseAlertBar : MonoBehaviour
{
    [Header("Alert Bar")]
    [SerializeField] float alertBar = 0;
    public enum AlertBarStates
    {
        IsNotDraining,
        IsDraining,
    }

    private AlertBarStates currentState;

    private void AlertBarController()
    {
        if(ControllerIsRunning == null)
        {
            ControllerIsRunning = ControllerRunner();
            StartCoroutine(ControllerIsRunning);
        }
    }

    private void AlertBarAdder(float value)
    {
        alertBar += value;
    }

    private void StartDecreasing()
    {
        currentState = AlertBarStates.IsDraining;

    }

    private void StopDecreasing()
    {
        currentState = AlertBarStates.IsNotDraining;
    }

    IEnumerator ControllerIsRunning;
    IEnumerator ControllerRunner()
    {
        if(currentState == AlertBarStates.IsDraining)
        {
            while (currentState == AlertBarStates.IsDraining)
            {
                if (alertBar <= 0)
                {

                }
                else
                {
                    alertBar -= 1;
                }
                yield return null;
            }
        }
        else
        {
            StopCoroutine(ControllerIsRunning);
            ControllerIsRunning = null;
        }
        
    }
}
