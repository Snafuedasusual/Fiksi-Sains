using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityHealthController : MonoBehaviour, IHealthInterface
{
    [Header("ScriptableObjects")]
    [SerializeField] protected EntityBaseHealthSO baseHealth;

    [Header("Variables")]
    [SerializeField] protected float health;


    private void Start()
    {
        health = baseHealth.health;
    }


    public virtual event EventHandler<HealthBarToUIArgs> HealthBarToUI;
    public class HealthBarToUIArgs : EventArgs { public float healthBarValue; }

    public virtual event EventHandler<SendDmgToLogicArgs> SendDmgToLogic;
    public class SendDmgToLogicArgs : EventArgs { public float currentHealth;  public float dmg; public Transform dmgSender; public float knckBckPwr; }
    public virtual void DealDamage(float damage, Transform dmgSender, float knckBckPwr)
    {
        if(health > 0)
        {
            health -= damage;
        }
        //baseHealth.OnHealthChangedEvent(health);
        SendDmgToLogic?.Invoke(this, new SendDmgToLogicArgs { currentHealth = health, dmg = damage, dmgSender = dmgSender, knckBckPwr = knckBckPwr });
        HealthBarToUI?.Invoke(this, new HealthBarToUIArgs { healthBarValue = health });
    }

    public virtual void Heal(float value)
    {
        if (health < 100)
        {
            health += value;
            if (health > 100)
            {
                health = baseHealth.maxHealth;
            }
            HealthBarToUI?.Invoke(this, new HealthBarToUIArgs { healthBarValue = health });
        }
    }

    public void ResetHealth()
    {
        health = baseHealth.health;
        HealthBarToUI?.Invoke(this, new HealthBarToUIArgs { healthBarValue = health });
    }
}
