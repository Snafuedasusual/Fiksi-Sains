using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemUses_Medkit : ItemUses, IMakeSounds
{
    [SerializeField] ItemSO itemSO;
    [SerializeField] MedkitUsesSO medKitUsesSO;

    private float currentProgress = 0f;
    private float maxProgress = 100f;

    private PlayerInput plrInp;
    private PlayerLogic plrLgc;
    private EntityHealthController healthCtrller;

    public void InitializeScript()
    {
        itemName = itemSO.itemName;
        itemEnum = itemSO.currentItemEnum;
        fireCooldown = 0f;
        knockBackPwr = 0f;
        damage = 0f;
        range = 0f;
        ammo = medKitUsesSO.ammo;
        maxAmmo = 0;
        itemUI = itemSO.uiIcon;
        itemDesc = itemSO.itemDescription;
        controller = medKitUsesSO.controller;
        soundRange = 0f;
    }

    public void DeInitializeScript()
    {
        throw new System.NotImplementedException();
    }

    private void OnEnable()
    {
        InitializeScript();
    }

    void OnDisable()
    {
        StopAllCoroutines();
    }

    Coroutine m1Debounce;
    IEnumerator StartM1Debounce()
    {
        var timer = 0f;
        var maxTimer = 0.1f;
        while(timer < maxTimer)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        m1Debounce = null;
    }
    bool debounce = false;
    public override void MainUse(bool isClicked, Transform source, float heightPos)
    {
        if(isClicked == true && debounce == false)
        {
            debounce = true;
            if (!source.TryGetComponent(out PlayerInput inp)) return;
            if (!source.TryGetComponent(out PlayerLogic lgc)) return;
            if (!source.TryGetComponent(out EntityHealthController healthController)) return;
            if (healthController.GetCurrentHealth() <= 0 || healthController.GetCurrentHealth() == healthController.GetCurrentMaxHealth()) return;
            plrLgc = lgc;
            plrInp = inp;
            healthCtrller = healthController;
            if (HealingProcess != null)
            {
                StopCoroutine(HealingProcess);
                HealingProcess = null;
                WaitBarManager.instance.DeactivateWaitBar();
                plrLgc.plrState = PlayerLogic.PlayerStates.Idle;
                RequestStopAudioSource(audSrc);
            }
            else
            {
                HealingProcess = StartCoroutine(StartHealingProcess());
            }
        }
        else if(isClicked == true && debounce == true)
        {

        }
        else if(isClicked == false)
        {
            debounce = false;
        }
    }

    Coroutine HealingProcess;
    IEnumerator StartHealingProcess()
    {
        AttackSound();
        currentProgress = 0f;
        WaitBarManager.instance.ActivateWaitBar();
        WaitBarManager.instance.UpdateBarValue(0f);
        var timer = 0f;
        var healRate = medKitUsesSO.healRate;
        plrLgc.plrState = PlayerLogic.PlayerStates.InteractingHold;
        while (currentProgress < maxProgress)
        {
            timer = 0f;
            while(timer < healRate)
            {
                plrLgc.Mouse1PlayAnim();
                if (plrInp.GetInputDir() != Vector2.zero) { plrLgc.plrState = PlayerLogic.PlayerStates.Idle; WaitBarManager.instance.DeactivateWaitBar(); HealingProcess = null; RequestStopAudioSource(audSrc);  yield break; }
                timer += Time.deltaTime;
                yield return null;
            }
            currentProgress++;
            WaitBarManager.instance.UpdateBarValue(currentProgress);
        }
        HealHealth();
        plrLgc.plrState = PlayerLogic.PlayerStates.Idle;
        WaitBarManager.instance.DeactivateWaitBar();
        HealingProcess = null;
        RequestStopAudioSource(audSrc);
        Destroy(gameObject);
    }

    private void HealHealth()
    {
        if (healthCtrller == null) return;
        var lostHealth = healthCtrller.GetCurrentMaxHealth() - healthCtrller.GetCurrentHealth();
        if(lostHealth <= ammo)
        {
            healthCtrller.Heal(ammo);
        }
        else
        {
            healthCtrller.Heal(lostHealth);
        }
    }

    public override void AttackSound()
    {
        audSrc.loop = true;
        if (medKitUsesSO.attackClips == null) return;
        if (medKitUsesSO.attackClips.Length <= 0) return;
        if (medKitUsesSO.attackClips.Length == 1) RequestPlaySFXAudioClip(audSrc, medKitUsesSO.attackClips[0]);
        else
        {
            var selectedAudioClip = Random.Range(0, medKitUsesSO.attackClips.Length);
            RequestPlaySFXAudioClip(audSrc, medKitUsesSO.attackClips[selectedAudioClip]);
        }
    }

    public void RequestPlaySFXAudioClip(AudioSource audSrc, AudioClip audClip)
    {
        SFXManager.instance.PlayAudio(audSrc, audClip);
    }

    public void RequestStopAudioSource(AudioSource audSrc)
    {
        SFXManager.instance.StopAudio(audSrc);
    }
}
