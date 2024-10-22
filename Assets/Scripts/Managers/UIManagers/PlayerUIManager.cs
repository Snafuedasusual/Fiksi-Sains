using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIManager : MonoBehaviour
{
    public static PlayerUIManager instance;
    [Header("Script References")]
    [SerializeField] PlayerToUI plrToUI;

    [SerializeField] GameObject uiElement;

    [Header("Health Components")]
    [SerializeField] GameObject healthComponent;
    [SerializeField] RawImage bloodEdges;
    [SerializeField] RawImage bloodOverlay;
    [SerializeField] float bloodEdgesMaxAlpha;
    [SerializeField] float bloodOverlayMaxAlpha;
    [SerializeField] private float maxHealth;
    [SerializeField] private float currentHealth;

    [Header("Visibility Components")]
    [SerializeField] GameObject visibiltyComponent;
    [SerializeField] Slider sliderVis;

    [Header("StaminaComponents")]
    [SerializeField] GameObject staminaComponent;
    [SerializeField] Slider bar1;
    [SerializeField] Slider bar2;
    [SerializeField] Image bar1Image;
    [SerializeField] Image bar2Image;
    [SerializeField] Image circle;

    [Header("ObjectiveText")]
    [SerializeField] GameObject objText;

    private float bar1ImageAlpha;
    private float bar2ImageAlpha;
    private float circleImageAlpha;

    public float GetHealthAlpha()
    {
        if (currentHealth == maxHealth)
        {
            return 0f;
        }
        else
        {
            var healthAlpha = maxHealth - currentHealth;
            return healthAlpha;
        }
    }
    private float currentVisibility;
    public float GetCurrentVisibility() { return currentVisibility; }
    private float currentStamina;
    private float GetCurrentStamina() { return currentStamina; }

    [Header("Interaction Notification")]
    [SerializeField] GameObject interactionNotifObj;
    [SerializeField] TextMeshProUGUI interactionNotif;

    private void Awake()
    {
        if(instance != null && instance != this)
        {

        }
        else
        {
            instance = this;
        }
    }

    private void InitializeScript()
    {
        plrToUI.SendHealthInfoToPlayerUI += SendHealthInfoToPlayerUIReceiver;
        plrToUI.SendStaminaInfoToPlayerUI += SendStaminaInfoToPlayerUIReceiver;
        plrToUI.SendVisibilityInfoToPlayerUI += SendVisibilityInfoToPlayerUIReceiver;
        plrToUI.SendInteractionInfoToPlayerUI += SendInteractionInfoToPlayerUIReceiver;
        plrToUI.SendNullifyStateEvent += SendNullifyStateEventReceiver;
    }


    private void DeInitializeScript()
    {
        plrToUI.SendHealthInfoToPlayerUI -= SendHealthInfoToPlayerUIReceiver;
        plrToUI.SendStaminaInfoToPlayerUI -= SendStaminaInfoToPlayerUIReceiver;
        plrToUI.SendVisibilityInfoToPlayerUI -= SendVisibilityInfoToPlayerUIReceiver;
        plrToUI.SendInteractionInfoToPlayerUI -= SendInteractionInfoToPlayerUIReceiver;
    }

    public void ActivateUI()
    {
        uiElement.SetActive(true);
    }

    public void DeActivateUI()
    {
        uiElement.SetActive(false);
    }

    private void OnEnable()
    {
        InitializeScript();
        bar1ImageAlpha = bar1Image.color.a;
        bar2ImageAlpha = bar2Image.color.a;
        circleImageAlpha = circle.color.a;
    }
    private void OnDisable()
    {
        DeInitializeScript();
    }
    private void OnDestroy()
    {
        DeInitializeScript();
    }

    private void SendNullifyStateEventReceiver(object sender, PlayerToUI.SendNullifyStateEventArgs e)
    {
        if(e.nullifyState == true)
        {
            healthComponent.gameObject.SetActive(false);
            visibiltyComponent.gameObject.SetActive(false);
            staminaComponent.gameObject.SetActive(false);
            objText.gameObject.SetActive(false);
            interactionNotifObj.gameObject.SetActive(false);
        }
        else
        {
            healthComponent.gameObject.SetActive(true);
            visibiltyComponent.gameObject.SetActive(true);
            staminaComponent.gameObject.SetActive(true);
            objText.gameObject.SetActive(true);
            interactionNotifObj.gameObject.SetActive(true);
        }
    }


    private void SendHealthInfoToPlayerUIReceiver(object sender, PlayerToUI.SendHealthInfoToPlayerUIArgs e)
    {
        //health.text = e.plrHealth.ToString();
        currentHealth = e.plrHealth;
        UpdateBloodAlpha(currentHealth);
    }

    private void UpdateBloodAlpha(float health)
    {
        var healthLost = maxHealth - health;
        if (healthLost == 0) 
        {
            bloodEdges.color = new Color(bloodEdges.color.r, bloodEdges.color.g, bloodEdges.color.b, 0f);
            bloodOverlay.color = new Color(bloodOverlay.color.r, bloodOverlay.color.g, bloodOverlay.color.b, 0f);
            return;
        }
        else
        {
            CalculateBloodEdgesAlpha(healthLost);
            CalculateBloodOverlayAlpha(healthLost);
        }
    }


    private void CalculateBloodEdgesAlpha(float healthLost)
    {
        var calculatedAlpha = (healthLost/100) * bloodEdgesMaxAlpha;
        var calculateRelativeToMaxAlpha = calculatedAlpha / 225;
        bloodEdges.color = new Color(bloodEdges.color.r, bloodEdges.color.g, bloodEdges.color.b, calculateRelativeToMaxAlpha);
    }
    
    private void CalculateBloodOverlayAlpha(float healthLost)
    {
        var calculatedAlpha = (healthLost / 100) * bloodOverlayMaxAlpha;
        var calculateRelativeToMaxAlpha = calculatedAlpha / 225;
        bloodOverlay.color = new Color(bloodOverlay.color.r, bloodOverlay.color.g, bloodOverlay.color.b, calculateRelativeToMaxAlpha);
    }

    private void SendStaminaInfoToPlayerUIReceiver(object sender, PlayerToUI.SendStaminaInfoToPlayerUIArgs e)
    {
        if(IsStartFadingOut != null) { StopCoroutine(IsStartFadingOut); IsStartFadingOut = null; IsStartFadingIn = StartCoroutine(StartFadingIn()); }
        if (circleImageAlpha != 1f && bar2ImageAlpha != 1f && bar1ImageAlpha != 1f && IsStartFadingIn == null) { IsStartFadingIn = StartCoroutine(StartFadingIn()); }
        else if(circleImageAlpha != 1f && bar2ImageAlpha != 1f && bar1ImageAlpha != 1f && IsStartFadingIn != null) { }
        //stamina.text = e.plrStamina.ToString();
        bar1.value = e.plrStamina;
        bar2.value = e.plrStamina;
        currentStamina = e.plrStamina;
        CheckIfStaminaFull();
    }

    Coroutine IsStartFadingIn;
    IEnumerator StartFadingIn()
    {
        for (float i = bar1ImageAlpha; i < 1f; i += Time.deltaTime)
        {

            bar1Image.color = new Color(bar1Image.color.r, bar1Image.color.g, bar1Image.color.b, i);
            bar2Image.color = new Color(bar2Image.color.r, bar2Image.color.g, bar2Image.color.b, i);
            circle.color = new Color(circle.color.r, circle.color.g, circle.color.b, i);
            yield return null;
        }
        bar1Image.color = new Color(bar1Image.color.r, bar1Image.color.g, bar1Image.color.b, 1f);
        bar2Image.color = new Color(bar2Image.color.r, bar2Image.color.g, bar2Image.color.b, 1f);
        circle.color = new Color(circle.color.r, circle.color.g, circle.color.b, 1f);
        bar1ImageAlpha = bar1Image.color.a;
        bar2ImageAlpha = bar2Image.color.a;
        circleImageAlpha = circle.color.a;
        IsStartFadingIn = null;
    }

    Coroutine IsStartFadingOut;
    IEnumerator StartFadingOut()
    {
        var waitTime = 0f;
        var waitRate = 0.75f;
        while(waitTime < waitRate)
        {
            waitTime += Time.deltaTime;
            yield return null;
        }
        if(IsStartFadingIn != null)
        {
            waitTime = 0f;
            while (waitTime < waitRate)
            {
                waitTime += Time.deltaTime;
                yield return null;
            }
        }
        for(float i = bar1ImageAlpha; i > 0f; i -= Time.deltaTime)
        {
            
            bar1Image.color = new Color(bar1Image.color.r, bar1Image.color.g, bar1Image.color.b, i);
            bar2Image.color = new Color(bar2Image.color.r, bar2Image.color.g, bar2Image.color.b, i);
            circle.color = new Color(circle.color.r, circle.color.g, circle.color.b, i);
            yield return null;
        }
        bar1Image.color = new Color(bar1Image.color.r, bar1Image.color.g, bar1Image.color.b, 0f);
        bar2Image.color = new Color(bar2Image.color.r, bar2Image.color.g, bar2Image.color.b, 0f);
        circle.color = new Color(circle.color.r, circle.color.g, circle.color.b, 0f);
        bar1ImageAlpha = bar1Image.color.a;
        bar2ImageAlpha = bar2Image.color.a;
        circleImageAlpha = circle.color.a;
        IsStartFadingOut = null;
    }
    private void CheckIfStaminaFull()
    {
        if(bar1.value == bar1.maxValue && bar2.value == bar2.maxValue)
        {
            if (IsStartFadingOut != null) return;
            IsStartFadingOut = StartCoroutine(StartFadingOut());
        }
    }






    private void SendVisibilityInfoToPlayerUIReceiver(object sender, PlayerToUI.SendVisibiltiyInfoToPlayerUIArgs e)
    {
        var newVis = Mathf.RoundToInt(e.plrVis);
        sliderVis.value = e.plrVis;
    }

    private void SendInteractionInfoToPlayerUIReceiver(object sender, PlayerToUI.SendInteractionInfoToPlayerUIArgs e)
    {
        if(e.target == null)
        {
            interactionNotifObj.SetActive(false);
        }
        else
        {
            interactionNotifObj.SetActive(true);
            if (e.notif == string.Empty) return;
            interactionNotif.text = e.notif;
        }
    }

}
