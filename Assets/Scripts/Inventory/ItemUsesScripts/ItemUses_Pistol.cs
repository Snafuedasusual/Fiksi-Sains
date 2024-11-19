using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static Unity.VisualScripting.Member;

public class ItemUses_Pistol : ItemUses, IInitializeScript, IMakeSounds
{
    [Header("ScriptableObjects")]
    [SerializeField] ItemSO itemSO;
    [SerializeField] FirearmUsesSO itemUsesSO;

    [SerializeField] Light light;


    bool click_debounce = false;



    public void InitializeScript()
    {
        itemName = itemSO.itemName;
        itemEnum = itemSO.currentItemEnum;
        fireCooldown = itemUsesSO.coolDown;
        knockBackPwr = itemUsesSO.knckBckPwr;
        damage = itemUsesSO.damage;
        range = itemUsesSO.range;
        maxAmmo = itemUsesSO.ammo;
        itemUI = itemSO.uiIcon;
        itemDesc = itemSO.itemDescription;
        soundRange = itemUsesSO.soundRange;
        light.enabled = false;
    }

    public void DeInitializeScript()
    {
        itemName = itemSO.itemName;
        fireCooldown = itemUsesSO.coolDown;
        knockBackPwr = itemUsesSO.knckBckPwr;
        damage = itemUsesSO.damage;
        range = itemUsesSO.range;
        maxAmmo = itemUsesSO.ammo;
    }

    private void OnEnable()
    {
        if(oneTimeActivation == false)
        {
            InitializeScript();
            oneTimeActivation = true;
        }
        else
        {

        }
    }

    IEnumerator HasFired;
    private IEnumerator Cooldown()
    {
        var coolDownTime = 0f;
        var coolDownRate = fireCooldown;
        light.enabled = true;
        while (coolDownTime < coolDownRate)
        {
            if(coolDownTime > 0.2) light.enabled = false;
            if (playerLogic != null) playerLogic.MakeSound(soundRange);
            coolDownTime += Time.deltaTime;
            yield return null;
        }
        if (playerLogic != null) playerLogic.MakeSound(0);
        playerLogic = null;
        HasFired = null;
    }

    Transform surce;
    float height;
    public override void MainUse(bool isClicked, Transform source, float heightPos)
    {
        if (isClicked == true && click_debounce == false && HasFired == null && ammo > 0)
        {
            playerLogic = source.TryGetComponent(out PlayerLogic lgc) ? lgc : null;
            if (playerLogic != null) { playerLogic.Mouse1PlayAnim(); AttackSound(); }
            HasFired = Cooldown();
            StartCoroutine(HasFired);
            click_debounce = true;
            ammo--;
            surce = playerLogic.AttackSource();
            height = heightPos;
            if (RotaryHeart.Lib.PhysicsExtension.Physics.Raycast(surce.position, surce.forward, out RaycastHit hit, range,RotaryHeart.Lib.PhysicsExtension.PreviewCondition.Editor))
            {
                if(hit.transform.gameObject.TryGetComponent<IHealthInterface>(out IHealthInterface Enemy))
                {
                    Enemy.DealDamage(damage, source, knockBackPwr);
                }
                else
                {
                    
                }
            }
        }
        if(ammo <= 0)
        {
            //Destroy(gameObject);
        }
        if (click_debounce == true && isClicked == true)
        {

        }
        else if(isClicked == false)
        {
            click_debounce = false;
        }

    }


    public override void RefillAmmo(int newAmmo)
    {
        if(ammo < maxAmmo)
        {
            ammo += newAmmo;
            if(ammo > maxAmmo)
            {
                ammo = maxAmmo;
            }
        }
        else
        {
            return;
            // Ammo is at max.
        }
    }



    public override void DropItem()
    {
        var rotation = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, 90f);
        var droppedItem = Instantiate(itemUsesSO.itemWorldPrefab, GameManagers.instance.GetCurrentSection().transform);
        if(Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 1000f, placesToDrop))
        {
            droppedItem.transform.eulerAngles = rotation;
            droppedItem.transform.position = hit.point;
        }
    }
    private void Update()
    {

    }

    private void OnDisable()
    {
        DeInitializeScript();
    }

    public override void AttackSound()
    {
        if (itemUsesSO.attackClips == null) return;
        if (itemUsesSO.attackClips.Length <= 0) return;
        if (itemUsesSO.attackClips.Length == 1) RequestPlaySFXAudioClip(audSrc, itemUsesSO.attackClips[0]);
        else
        {
            var selectedAudioClip = Random.Range(0, itemUsesSO.attackClips.Length);
            RequestPlaySFXAudioClip(audSrc, itemUsesSO.attackClips[selectedAudioClip]);
        }
    }

    public void RequestPlaySFXAudioClip(AudioSource audSrc, AudioClip audClip)
    {
        SFXManager.instance.PlayAudio(audSrc, audClip);
    }
}
