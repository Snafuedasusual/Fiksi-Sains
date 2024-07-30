using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "EntityBaseHealth", menuName = "EntityStats/EntityHealth")]
public class EntityBaseHealthSO : ScriptableObject
{
    public float maxHealth;
    public float health;

    public event EventHandler OnHealthChanged;
    public class OnHealthChangedArgs : EventArgs { public float currentHealth; }


    private void Awake()
    {
        health = maxHealth;
    }

    public void HealthHeal(float value)
    {
        if(health < 100)
        {
            health += value;
            if(health > maxHealth)
            {
                health = maxHealth;
            }
        }
    }

    public void HealthDamaged(float value)
    {
        if(health > 0)
        {
            health -= value;
        }
    }

    public void OnHealthChangedEvent(float currentHealth)
    {
        OnHealthChanged?.Invoke(this, new OnHealthChangedArgs { currentHealth = currentHealth });
    }
}
