using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityHealthController : MonoBehaviour, IInflictDamage
{
    [Header("ScriptableObjects")]
    [SerializeField] EntityBaseHealth baseHealth;

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
        baseHealth.HealthHeal(-damage);
        health = baseHealth.health;
        SendDmgToLogic?.Invoke(this, new SendDmgToLogicArgs { currentHealth = health, dmg = damage, dmgSender = dmgSender, knckBckPwr = knckBckPwr });
    }

    public void Heal(float value)
    {
        baseHealth.HealthHeal(value);
        health = baseHealth.health;
    }

    public float GetHealth()
    {
        return health;
    }
}
