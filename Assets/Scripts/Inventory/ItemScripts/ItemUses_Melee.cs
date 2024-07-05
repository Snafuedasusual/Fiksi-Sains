using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemUses_Melee : ItemUses
{
    [SerializeField] private string itemName;
    [SerializeField] private float atkRange;
    [SerializeField] private float dmg;
    [SerializeField] private float knckBckPwr;
    public override void MainUse(bool isClicked, Transform source, Transform plr)
    {
        if(Cooldown == null)
        {
            StartCoroutine(CoolingDown());
            Collider[] listColliders = RotaryHeart.Lib.PhysicsExtension.Physics.OverlapBox(transform.position + Vector3.up * 1f, new Vector3(1f, 0.5f, 1.5f), Quaternion.LookRotation(transform.forward), RotaryHeart.Lib.PhysicsExtension.PreviewCondition.Both);
            foreach (Collider collider in listColliders)
            {
                Vector3 direction = collider.transform.position - plr.position;
                float distance = Vector3.Distance(collider.transform.position, plr.position);
                float dotDir = Vector3.Dot(plr.forward, direction.normalized);
                if (dotDir > 0.3f && distance <= atkRange)
                {
                    if (collider.TryGetComponent<IInflictDamage>(out IInflictDamage Enemy))
                    {
                        Enemy.DealDamage(dmg, plr, knckBckPwr);
                    }
                }
            }
        } 
    }

    IEnumerator Cooldown;
    IEnumerator CoolingDown()
    {
        float meleeTime = 0f;
        float meleeRate = 0.3f;
        if(Cooldown != null)
        {

        }
        else
        {
            Cooldown = CoolingDown();
            while(meleeTime < meleeRate)
            {
                meleeTime += Time.deltaTime;
                yield return 0;
            }
            Cooldown = null;
        }
        Cooldown = null;
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    public override string GetName()
    {
        return itemName;
    }
}
