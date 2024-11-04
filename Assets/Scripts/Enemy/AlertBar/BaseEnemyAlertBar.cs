using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEnemyAlertBar : MonoBehaviour
{
    [Header("Alert Bar")]
    [SerializeField] float alertBar = 0;

    [Header("Script References")]
    [SerializeField] BaseEnemyLogic baseEnemyLogic;


    public enum AlertBarStates
    {
        IsNotDraining,
        IsDraining,
    }

    private AlertBarStates currentState;

    private void Start()
    {
        baseEnemyLogic.SendEventToAlertBarScr += SendEventToAlertBarScrReceiver;
    }


    private void AlertBarController()
    {
        if(ControllerIsRunning == null)
        {
            ControllerIsRunning = ControllerRunner();
            StartCoroutine(ControllerIsRunning);
        }
    }

    private void SendEventToAlertBarScrReceiver(object sender, BaseEnemyLogic.SendEventToAlertBarScrArgs e)
    {
        if (e.adder > 0f)
        {
            AlertBarAdder(e.adder);
        }
        else if(e.adder <= 0f)
        {
            StartDecreasing();
        }
    }

    private void AlertBarAdder(float value)
    {
        if(alertBar < 100)
        {
            alertBar += value;
            StopDecreasing();
        }
        else if(alertBar > 100)
        {
            var alertBarFixer = alertBar - 100;
            alertBar -= alertBarFixer;
        }
        else
        {

        }
        AlertBarIsNotEmpty?.Invoke(this, EventArgs.Empty);
    }

    private void StartDecreasing()
    {
        currentState = AlertBarStates.IsDraining;
        AlertBarController();
    }

    private void StopDecreasing()
    {
        currentState = AlertBarStates.IsNotDraining;
    }

    public event EventHandler AlertBarIsNotEmpty;
    public event EventHandler AlertBarIsEmpty;


    IEnumerator ControllerIsRunning;
    IEnumerator ControllerRunner()
    {
        var drainTime = 0f;
        var drainRate = 0.45f;
        if(currentState == AlertBarStates.IsDraining)
        {
            while (currentState == AlertBarStates.IsDraining)
            {
                drainTime = 0f;
                while(drainTime < drainRate)
                {
                    drainTime += Time.deltaTime;
                    yield return null;
                }
                if (alertBar <= 0)
                {
                    StopCoroutine(ControllerIsRunning);
                    ControllerIsRunning = null;
                    AlertBarIsEmpty?.Invoke(this, EventArgs.Empty);
                }
                if(currentState != AlertBarStates.IsDraining)
                {
                    StopCoroutine(ControllerIsRunning);
                    ControllerIsRunning = null;
                }
                else if(alertBar > 0)
                {
                    alertBar -= 1;
                }
            }
        }
        else
        {
            StopCoroutine(ControllerIsRunning);
            ControllerIsRunning = null;
        }
        
    }

    public void ResetEnemyAlertBar()
    {
        alertBar = 0f;
    }
}
