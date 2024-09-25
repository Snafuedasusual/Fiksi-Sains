using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.Rendering;

public class PlayerToUI : MonoBehaviour, IInitializeScript
{
    [Header("Player Script References")]
    [SerializeField] PlayerLogic plrLogic;
    [SerializeField] EntityHealthController healthController;
    [SerializeField] EntityVisibilityController visibilityController;

    [SerializeField] InventoryMenuManager inventoryMenuManager;

    public void InitializeScript()
    {
        plrLogic.StaminaBarToUI += StaminaBarToUIReceiver;
        healthController.HealthBarToUI += HealthBarToUIReceiver;
        visibilityController.VisBarToUI += VisBarToUIReceiver;
        plrLogic.InteractNotif += InteractNotifReceiver;
        inventoryMenuManager.EquipItemEvent += EquipItemEventReceiver;
        plrLogic.NullifyStateEvent += NullifyStateEventReceiver;
    }


    public void DeInitializeScript()
    {
        plrLogic.StaminaBarToUI -= StaminaBarToUIReceiver;
        healthController.HealthBarToUI -= HealthBarToUIReceiver;
        visibilityController.VisBarToUI -= VisBarToUIReceiver;
        plrLogic.InteractNotif -= InteractNotifReceiver;
        plrLogic.NullifyStateEvent -= NullifyStateEventReceiver;
    }



    private void OnEnable()
    {
        InitializeScript();
    }



    private void Start()
    {
        InitializeScript();
    }

    private void OnDestroy()
    {
        DeInitializeScript();
    }

    private void OnDisable()
    {
        DeInitializeScript();
    }


    //Send Player Events
    public event EventHandler<SendNullifyStateEventArgs> SendNullifyStateEvent;
    public class SendNullifyStateEventArgs : EventArgs { public bool nullifyState; }
    public event EventHandler<SendStaminaInfoToPlayerUIArgs> SendStaminaInfoToPlayerUI;
    public class SendStaminaInfoToPlayerUIArgs : EventArgs {public float plrStamina;}
    
    public event EventHandler<SendHealthInfoToPlayerUIArgs> SendHealthInfoToPlayerUI;
    public class SendHealthInfoToPlayerUIArgs : EventArgs { public float plrHealth; }
    
    public event EventHandler<SendVisibiltiyInfoToPlayerUIArgs> SendVisibilityInfoToPlayerUI;
    public class SendVisibiltiyInfoToPlayerUIArgs : EventArgs { public float plrVis; }

    public event EventHandler<SendInteractionInfoToPlayerUIArgs> SendInteractionInfoToPlayerUI;
    public class SendInteractionInfoToPlayerUIArgs : EventArgs { public Transform target; public string notif; }
    
    public event EventHandler OpenInventoryScreen;


    private void NullifyStateEventReceiver(object sender, PlayerLogic.NullifyStateEventArgs e)
    {
        SendNullifyStateEvent?.Invoke(this, new SendNullifyStateEventArgs { nullifyState = e.nullifyState });
    }


    private void StaminaBarToUIReceiver(object sender, PlayerLogic.StaminaBarToUIArgs e)
    {
        SendStaminaInfoToPlayerUI?.Invoke(this, new SendStaminaInfoToPlayerUIArgs { plrStamina = e.staminaBarValue });
    }

    private void HealthBarToUIReceiver(object sender, EntityHealthController.HealthBarToUIArgs e)
    {
        SendHealthInfoToPlayerUI?.Invoke(this, new SendHealthInfoToPlayerUIArgs { plrHealth = e.healthBarValue});
    }

    private void VisBarToUIReceiver(object sender, EntityVisibilityController.VisBarToUIArgs e)
    {
        SendVisibilityInfoToPlayerUI?.Invoke(this, new SendVisibiltiyInfoToPlayerUIArgs { plrVis = e.visibilityBarValue});
    }

    private void InteractNotifReceiver(object sender, PlayerLogic.InteractNotifArgs e)
    {
        SendInteractionInfoToPlayerUI?.Invoke(this, new SendInteractionInfoToPlayerUIArgs { target = e.target, notif = e.notif });
    }

    private void OnOpenInventoryReceiver(object sender, EventArgs e)
    {
        OpenInventoryScreen?.Invoke(this, EventArgs.Empty);
    }

    //Receive UI Events
    public event EventHandler<EquipItemEventSenderEventArgs> EquipItemEventSenderEvent;
    public class EquipItemEventSenderEventArgs : EventArgs { public GameObject item; public string notif; }
    private void EquipItemEventReceiver(object sender, InventoryMenuManager.EquipItemEventArgs e)
    {
        EquipItemEventSenderEvent?.Invoke(this, new EquipItemEventSenderEventArgs { item = e.item });
    }
}
