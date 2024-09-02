using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIInputDetectir : MonoBehaviour, IInitializeScript
{
    [Header("ScriptReferences")]
    [SerializeField] PlayerInput plrInp;


    public void DeInitializeScript()
    {
        plrInp.EscInputEvent -= EscInputEventReceiver;
        plrInp.OnTabInput -= OnTabInputReceiver;
    }

    public void InitializeScript()
    {
        plrInp.EscInputEvent += EscInputEventReceiver;
        plrInp.OnTabInput += OnTabInputReceiver;
        plrInp.OnEInputEvent += OnEInputEventReceiver;
    }


    private void Start()
    {
        InitializeScript();
    }
    private void OnEnable()
    {
        InitializeScript();
    }

    private void OnDisable()
    {
        DeInitializeScript();
    }
    private void OnDestroy()
    {
        DeInitializeScript();
    }

    IEnumerator IsEscDebounce;
    IEnumerator EscDebounce()
    {
        var debTime = 0f;
        var debRate = 0.3f;
        while(debTime < debRate)
        {
            debTime += Time.deltaTime;
            yield return null;
        }
        IsEscDebounce = null;
    }
    private void EscInputEventReceiver(object sender, System.EventArgs e)
    {
        if(IsEscDebounce == null)
        {
            GameManagers.instance.EscInputReceiver();
        }   
    }


    


    IEnumerator IsTabDebounce;
    IEnumerator TabDebounce()
    {
        var debTime = 0f;
        var debRate = 0.3f;
        while (debTime < debRate)
        {
            debTime += Time.deltaTime;
            yield return null;
        }
        IsTabDebounce = null;
    }
    private void OnTabInputReceiver(object sender, System.EventArgs e)
    {
        if(IsTabDebounce == null)
        {
            InventoryMenuManager.instance.InventoryMenuController();
        }
    }


    public event EventHandler OnEInputEventSender;
    private void OnEInputEventReceiver(object sender, System.EventArgs e)
    {
        OnEInputEventSender?.Invoke(sender, e);
    }


}
