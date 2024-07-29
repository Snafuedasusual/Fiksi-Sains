using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "EntityBaseHealth", menuName = "EntityStats/EntityHealth")]
public class EntityBaseHealth : ScriptableObject
{
    public float maxHealth;
    public float health;

    public event EventHandler OnHealthChanged;
    public class OnHealthChangedArgs : EventArgs { public float value; }


    private void Awake()
    {
        health = maxHealth;
    }

    public void HealthChanged(float value)
    {
        if(health < 100 || health > 0)
        {
            health += value;
            if(health > maxHealth)
            {
                health = maxHealth;
            }
            OnHealthChanged?.Invoke(this, new OnHealthChangedArgs { value = value });
        }
        else if(health > 100 || health <= 0)
        {

        }
    }
}
