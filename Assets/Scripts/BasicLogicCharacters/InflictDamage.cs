using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface IInflictDamage
{
   public void Heal(float value)
   {

   }

   public void DealDamage(float damage, Transform dmgSender, float knckBckPwr)
   {

   }
   public void KnockBack(Transform sender, float knockBackPwr)
   {

   }

    public void CheckStatus(float health)
    {

    }
}
