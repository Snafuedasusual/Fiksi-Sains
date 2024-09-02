using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerUIManager : MonoBehaviour
{
    [Header("Script References")]
    [SerializeField] PlayerToUI plrToUI;

    [Header("Health Components")]
    [SerializeField] TextMeshProUGUI health;

    [Header("Visibility Components")]
    [SerializeField] TextMeshProUGUI visibility;

    [Header("StaminaComponents")]
    [SerializeField] TextMeshProUGUI stamina;

    [Header("Interaction Notification")]
    [SerializeField] TextMeshProUGUI interactionNotif;

    private void InitializeScript()
    {
        plrToUI.SendHealthInfoToPlayerUI += SendHealthInfoToPlayerUIReceiver;
        plrToUI.SendStaminaInfoToPlayerUI += SendStaminaInfoToPlayerUIReceiver;
        plrToUI.SendVisibilityInfoToPlayerUI += SendVisibilityInfoToPlayerUIReceiver;
        plrToUI.SendInteractionInfoToPlayerUI += SendInteractionInfoToPlayerUIReceiver;
    }


    private void DeInitializeScript()
    {
        plrToUI.SendHealthInfoToPlayerUI -= SendHealthInfoToPlayerUIReceiver;
        plrToUI.SendStaminaInfoToPlayerUI -= SendStaminaInfoToPlayerUIReceiver;
        plrToUI.SendVisibilityInfoToPlayerUI -= SendVisibilityInfoToPlayerUIReceiver;
        plrToUI.SendInteractionInfoToPlayerUI -= SendInteractionInfoToPlayerUIReceiver;
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

    private void SendHealthInfoToPlayerUIReceiver(object sender, PlayerToUI.SendHealthInfoToPlayerUIArgs e)
    {
        health.text = e.plrHealth.ToString();
    }

    private void SendStaminaInfoToPlayerUIReceiver(object sender, PlayerToUI.SendStaminaInfoToPlayerUIArgs e)
    {
        stamina.text = e.plrStamina.ToString();
    }

    private void SendVisibilityInfoToPlayerUIReceiver(object sender, PlayerToUI.SendVisibiltiyInfoToPlayerUIArgs e)
    {
        var newVis = Mathf.RoundToInt(e.plrVis);
        visibility.text = newVis.ToString();
    }

    private void SendInteractionInfoToPlayerUIReceiver(object sender, PlayerToUI.SendInteractionInfoToPlayerUIArgs e)
    {
        if(e.target == null)
        {
            interactionNotif.transform.gameObject.gameObject.SetActive(false);
        }
        else
        {
            interactionNotif.transform.gameObject.gameObject.SetActive(true);
        }
    }

}
