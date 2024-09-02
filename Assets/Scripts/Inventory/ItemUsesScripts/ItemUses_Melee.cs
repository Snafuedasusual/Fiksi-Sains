using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemUses_Melee : ItemUses
{
    public override void MainUse(bool isClicked, Transform source, float heightPos)
    {
        if(Cooldown == null)
        {
            var colliders = RotaryHeart.Lib.PhysicsExtension.Physics.OverlapBox(source.position + transform.up * heightPos, new Vector3(1f, 0.5f, 1f), Quaternion.identity, RotaryHeart.Lib.PhysicsExtension.PreviewCondition.Game);
            for(int i = 0; i < colliders.Length; i++)
            {
                IHealthInterface healthCtrl;
                var currentObj = colliders[i];
                var direction = currentObj.transform.position - source.position;
                var isNotObstructed = !Physics.Raycast(source.position + transform.up * heightPos, direction, range, ~characters);
                if (currentObj.transform.TryGetComponent(out healthCtrl) && currentObj.transform != source && isNotObstructed)
                {
                    healthCtrl.DealDamage(damage, source, knockBackPwr);
                }
                if(!currentObj.transform.TryGetComponent(out healthCtrl) && currentObj.transform != source && isNotObstructed)
                {
                    
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
}
