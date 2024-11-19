using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;

public class ItemUses_Knife : ItemUses, IInitializeScript, IMakeSounds
{
    [SerializeField] ItemSO itemSO;
    [SerializeField] KnifeUsesSO knifeUsesSO;


    public void InitializeScript()
    {
        itemName = itemSO.itemName;
        itemEnum = itemSO.currentItemEnum;
        fireCooldown = knifeUsesSO.coolDown;
        knockBackPwr = knifeUsesSO.knckBckPwr;
        damage = knifeUsesSO.damage;
        range = knifeUsesSO.range;
        maxAmmo = 0;
        itemUI = itemSO.uiIcon;
        itemDesc = itemSO.itemDescription;
        controller = knifeUsesSO.controller;
        soundRange = knifeUsesSO.soundRange;
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
        Cooldown = null;
        debounce = false;
    }

    bool debounce = false;
    public override void MainUse(bool isClicked, Transform source, float heightPos)
    {
        if(Cooldown == null && debounce == false && isClicked == true)
        {
            playerLogic = source.TryGetComponent(out PlayerLogic plr) ? plr : null;
            if(playerLogic != null) { playerLogic.Mouse1PlayAnim(); AttackSound(); }
            debounce = true;
            Cooldown = StartCoroutine(CoolingDown(fireCooldown));
            var colliders = RotaryHeart.Lib.PhysicsExtension.Physics.OverlapBox(playerLogic.AttackSource().position * heightPos, new Vector3(range, 0.5f, range), source.transform.rotation, RotaryHeart.Lib.PhysicsExtension.PreviewCondition.Editor);
            for(int i = 0; i < colliders.Length; i++)
            {
                IHealthInterface healthCtrl;
                var currentObj = colliders[i];
                var direction = currentObj.transform.position - source.position;
                var isNotObstructed = Physics.Raycast(source.position + transform.up * heightPos, direction, range);
                if (currentObj.transform.TryGetComponent(out healthCtrl) && currentObj.transform != source && isNotObstructed)
                {
                    healthCtrl.DealDamage(damage, source, knockBackPwr);
                }
                else if(!currentObj.transform.TryGetComponent(out healthCtrl) && currentObj.transform != source && isNotObstructed)
                {
                    
                }
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

    Coroutine Cooldown;
    IEnumerator CoolingDown(float coolDownRate)
    {
        float meleeTime = 0f;
        float meleeRate = coolDownRate;
        while(meleeTime < meleeRate)
        {
            if (playerLogic != null) playerLogic.MakeSound(soundRange);
            meleeTime += Time.deltaTime;
            yield return null;
        }
        if (playerLogic != null) playerLogic.MakeSound(0);
        playerLogic = null;
        Cooldown = null;
    }

    public override void AttackSound()
    {
        if (knifeUsesSO.attackClips == null) return;
        if (knifeUsesSO.attackClips.Length <= 0) return;
        if (knifeUsesSO.attackClips.Length == 1) RequestPlaySFXAudioClip(audSrc, knifeUsesSO.attackClips[0]);
        else
        {
            var selectedAudioClip = Random.Range(0, knifeUsesSO.attackClips.Length);
            RequestPlaySFXAudioClip(audSrc, knifeUsesSO.attackClips[selectedAudioClip]);
        }
    }

    public void RequestPlaySFXAudioClip(AudioSource audSrc, AudioClip audClip)
    {
        SFXManager.instance.PlayAudio(audSrc, audClip);
    }
}
