using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static Unity.VisualScripting.Member;

public class ItemUses_Pistol : ItemUses, IInitializeScript
{
    [Header("ScriptableObjects")]
    [SerializeField] ItemSO itemSO;
    [SerializeField] FirearmUsesSO itemUsesSO;

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

    private void Start()
    {
        InitializeScript();
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
        while (coolDownTime < coolDownRate)
        {
            coolDownTime += Time.deltaTime;
            yield return null;
        }
        HasFired = null;
    }

    Transform surce;
    float height;
    public override void MainUse(bool isClicked, Transform source, float heightPos)
    {
        if (isClicked == true && click_debounce == false && HasFired == null && ammo > 0)
        {
            var plrLgc = source.TryGetComponent(out PlayerLogic lgc) ? lgc : null;
            if (plrLgc != null) plrLgc.Mouse1PlayAnim();
            HasFired = Cooldown();
            StartCoroutine(HasFired);
            click_debounce = true;
            ammo--;
            surce = source;
            height = heightPos;
            if (RotaryHeart.Lib.PhysicsExtension.Physics.Raycast(source.position + transform.up * heightPos, source.forward, out RaycastHit hit, range,RotaryHeart.Lib.PhysicsExtension.PreviewCondition.Game))
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
}
