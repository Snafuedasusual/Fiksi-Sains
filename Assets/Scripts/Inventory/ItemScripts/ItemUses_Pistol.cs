using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemUses_Pistol : ItemUses
{
    [SerializeField] private string itemName;
    [SerializeField] private float fireCooldown;
    [SerializeField] private float knockBackPwr;
    [SerializeField] private float damage;

    public float crrntAmmo;

    bool click_debounce = false;

    public override string GetName()
    {
        return itemName;
    }

    IEnumerator HasFired = null;
    public override void MainUse(bool isClicked, Transform source, Transform plr)
    {
        if (isClicked == true && click_debounce == false && HasFired == null && crrntAmmo > 0)
        {
            StartCoroutine(Cooldown());
            click_debounce = true;
            crrntAmmo--;
            if (Physics.Raycast(source.position, source.forward, out RaycastHit hit, 30f))
            {

                if(hit.transform.gameObject.TryGetComponent<IInflictDamage>(out IInflictDamage Enemy))
                {
                    Enemy.DealDamage(damage, plr, knockBackPwr);
                }
            }
        }
        if(crrntAmmo <= 0)
        {
            Destroy(gameObject);
        }
        if (click_debounce == true && isClicked == true)
        {

        }
        else
        {
            click_debounce = false;
        }

    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }
    private IEnumerator Cooldown()
    {
        HasFired = Cooldown();
        yield return new WaitForSecondsRealtime(fireCooldown);
        HasFired = null;
    }

}
