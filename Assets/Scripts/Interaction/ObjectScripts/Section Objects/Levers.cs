using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Levers : MonoBehaviour, IInteraction
{
    [SerializeField] HandlerSection3 handler;

    IEnumerator IsInteractDebounce;

    public event EventHandler OnInteractActive;
    public event EventHandler OnInteractDeactive;

    IEnumerator InteractDebounce()
    {
        var debTime = 0f;
        var debRate = 0.1f;
        while(debTime < debRate)
        {
            debTime += Time.deltaTime;
            yield return null;
        }
        IsInteractDebounce = null;
        gameObject.SetActive(false);
    }
    public void OnInteract(Transform plr)
    {
        if(transform.parent.TryGetComponent(out LeverObjective leverObj) && IsInteractDebounce == null)
        {
            IsInteractDebounce = InteractDebounce();
            StartCoroutine(IsInteractDebounce);
            leverObj.InteractedLever();
        }
    }

    public void OnDetected(Transform plr)
    {

    }

    private void OnEnable()
    {
        IsInteractDebounce = null;
    }

    private void OnDisable()
    {
        IsInteractDebounce = null;
    }

    private void OnDestroy()
    {
        IsInteractDebounce = null;
    }

}
