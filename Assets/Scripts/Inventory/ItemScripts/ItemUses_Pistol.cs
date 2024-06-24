using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemUses_Pistol : ItemUses
{
    [SerializeField] private string itemName;
    [SerializeField] private float fireCooldown;
    [SerializeField] private float knockBackPwr;
    [SerializeField] private float damage;

    bool click_debounce = false;

    public override string GetName()
    {
        return itemName;
    }

    IEnumerator HasFired = null;
    public override void MainUse(bool isClicked, Transform source, Transform plr)
    {
        if (isClicked == true && click_debounce == false && HasFired == null)
        {
            StartCoroutine(Cooldown());
            click_debounce = true;
            if (Physics.Raycast(source.position, source.forward, out RaycastHit hit, 30f))
            {

                if(hit.transform.gameObject.TryGetComponent<EnemyScriptBase>(out EnemyScriptBase Enemy))
                {
                    Enemy.InflictDamage(plr, knockBackPwr, damage);
                }
            }
        }
        if (click_debounce == true && isClicked == true)
        {

        }
        else
        {
            click_debounce = false;
        }

    }

    private IEnumerator Cooldown()
    {
        HasFired = Cooldown();
        yield return new WaitForSecondsRealtime(fireCooldown);
        HasFired = null;
    }

}
