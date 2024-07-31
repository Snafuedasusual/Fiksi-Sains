using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flashlight : MonoBehaviour
{
    [SerializeField] PlayerLogic plrLogic;
    [SerializeField] GameObject flashLight;

    private void Start()
    {
        plrLogic.TurnFlashlightEvent += TurnFlashlightEventReceiver;
    }

    private void OnEnable()
    {
        plrLogic.TurnFlashlightEvent += TurnFlashlightEventReceiver;
    }

    private void TurnFlashlightEventReceiver(object sender, System.EventArgs e)
    {
        TurnFlashLight();
    }

    void TurnFlashLight()
    {
        if(IsDebounce == null)
        {
            IsDebounce = Debounce();
            StartCoroutine(IsDebounce);
            if (flashLight.activeSelf == false)
            {
                flashLight.SetActive(true);
            }
            else
            {
                flashLight.SetActive(false);
            }
        }
        
    }

    IEnumerator IsDebounce;
    IEnumerator Debounce()
    {
        var debTime = 0f;
        var debRate = 0.15f;
        while(debTime < debRate)
        {
            debTime += Time.deltaTime;
            yield return 0;
        }
        IsDebounce = null;
    }

    private void OnDisable()
    {
        plrLogic.TurnFlashlightEvent -= TurnFlashlightEventReceiver;
    }

    private void OnDestroy()
    {
        plrLogic.TurnFlashlightEvent -= TurnFlashlightEventReceiver;
    }

}
