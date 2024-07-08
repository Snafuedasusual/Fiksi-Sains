using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemUses_Melee : ItemUses
{
    [SerializeField] private string itemName;
    [SerializeField] private float atkRange;
    [SerializeField] private float dmg;
    [SerializeField] private float knckBckPwr;
    [SerializeField] Collider[] listColliders;
    public override void MainUse(bool isClicked, Transform source, Transform plr)
    {
        if(Cooldown == null && isClicked == true)
        {
            StartCoroutine(CoolingDown());
            listColliders = RotaryHeart.Lib.PhysicsExtension.Physics.OverlapBox(plr.position+ (plr.transform.up * 1.5f), new Vector3(2f, 0.5f, 2f), Quaternion.LookRotation(plr.forward), RotaryHeart.Lib.PhysicsExtension.PreviewCondition.Both);

            for(int i = 0; i < listColliders.Length; i++)
            {
                Vector3 direction = listColliders[i].transform.position - plr.position;
                float distance = Vector3.Distance(listColliders[i].transform.position, plr.position);
                float dotDir = Vector3.Dot(plr.forward, direction.normalized);
                if (dotDir > 0.3f && distance <= atkRange)
                {
                    if(Physics.Raycast(plr.position + Vector3.up * 0.75f, direction, out RaycastHit hit,atkRange))
                    {
                        if (hit.transform == listColliders[i].transform)
                        {
                            if(listColliders[i].transform != plr && listColliders[i].transform.TryGetComponent<IInflictDamage>(out IInflictDamage dealDmg))
                            {
                                dealDmg.DealDamage(dmg, plr, knckBckPwr);
                            }
                        }
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
