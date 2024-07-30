using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityHealthController : MonoBehaviour, IInflictDamage
{
    [Header("ScriptableObjects")]
    [SerializeField] EntityBaseHealthSO baseHealth;

    [Header("Variables")]
    [SerializeField] float health;


    private void Start()
    {
        health = baseHealth.health;
    }

    public event EventHandler<SendDmgToLogicArgs> SendDmgToLogic;
    public class SendDmgToLogicArgs : EventArgs { public float currentHealth;  public float dmg; public Transform dmgSender; public float knckBckPwr; }
    public void DealDamage(float damage, Transform dmgSender, float knckBckPwr)
    {
        if(health > 0)
        {
            health -= damage;
        }
        baseHealth.OnHealthChangedEvent(health);
        SendDmgToLogic?.Invoke(this, new SendDmgToLogicArgs { currentHealth = health, dmg = damage, dmgSender = dmgSender, knckBckPwr = knckBckPwr });

    }

    public void Heal(float value)
    {
        if (health < 100)
        {
            health += value;
            if (health > 100)
            {
                health = baseHealth.maxHealth;
            }
        }
    }
}
