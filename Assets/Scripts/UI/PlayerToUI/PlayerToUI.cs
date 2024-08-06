using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.Rendering;

public class PlayerToUI : MonoBehaviour
{
    [Header("Script References")]
    [SerializeField] PlayerLogic plrLogic;
    [SerializeField] EntityHealthController healthController;
    [SerializeField] EntityVisibilityController visibilityController;



    private void OnEnable()
    {
        plrLogic.StaminaBarToUI += StaminaBarToUIReceiver;
        healthController.HealthBarToUI += HealthBarToUIReceiver;
        visibilityController.VisBarToUI += VisBarToUIReceiver;
        plrLogic.InteractNotif += InteractNotifReceiver;
    }



    private void Start()
    {
        plrLogic.StaminaBarToUI += StaminaBarToUIReceiver;
        healthController.HealthBarToUI += HealthBarToUIReceiver;
        visibilityController.VisBarToUI += VisBarToUIReceiver;
        plrLogic.InteractNotif += InteractNotifReceiver;
    }

    private void OnDestroy()
    {
        plrLogic.StaminaBarToUI -= StaminaBarToUIReceiver;
        healthController.HealthBarToUI -= HealthBarToUIReceiver;
        visibilityController.VisBarToUI -= VisBarToUIReceiver;
        plrLogic.InteractNotif -= InteractNotifReceiver;
    }

    private void OnDisable()
    {
        plrLogic.StaminaBarToUI -= StaminaBarToUIReceiver;
        healthController.HealthBarToUI -= HealthBarToUIReceiver;
        visibilityController.VisBarToUI -= VisBarToUIReceiver;
        plrLogic.InteractNotif -= InteractNotifReceiver;
    }


    //Send Player Events
    public event EventHandler<SendStaminaInfoToPlayerUIArgs> SendStaminaInfoToPlayerUI;
    public class SendStaminaInfoToPlayerUIArgs : EventArgs {public float plrStamina;}
    
    public event EventHandler<SendHealthInfoToPlayerUIArgs> SendHealthInfoToPlayerUI;
    public class SendHealthInfoToPlayerUIArgs : EventArgs { public float plrHealth; }
    
    public event EventHandler<SendVisibiltiyInfoToPlayerUIArgs> SendVisibilityInfoToPlayerUI;
    public class SendVisibiltiyInfoToPlayerUIArgs : EventArgs { public float plrVis; }

    public event EventHandler<SendInteractionInfoToPlayerUIArgs> SendInteractionInfoToPlayerUI;
    public class SendInteractionInfoToPlayerUIArgs : EventArgs { public Transform target; }  


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
        SendInteractionInfoToPlayerUI?.Invoke(this, new SendInteractionInfoToPlayerUIArgs { target = e.target });
    }


    //Receive UI Events
}
