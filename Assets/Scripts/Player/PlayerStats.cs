using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerStats : MonoBehaviour
{
    //Current stats
    public float maxHealth;
    public float defaultDamage;
    private float _currentHealth;
    [FormerlySerializedAs("_currentDamage")] [HideInInspector]
    public float currentDamage;
    
    [Header("UI Elements")]
    public HealthBar healthBar;
    
    

    //I-Frames
    [Header("I-Frames")]
    public float invincibilityDuration;
    float _invincibilityTimer;
    private bool _isInvincible;
    
    void Start()
    {
        _currentHealth = maxHealth;
        currentDamage = defaultDamage;
        healthBar.SetMaxHealth(_currentHealth);
    }

    void Update()
    {
        if (_invincibilityTimer > 0)
        {
            _invincibilityTimer -= Time.deltaTime;
        }
        //If the invincibility timer has reached 0, set the invincibility flag to false
        else if (_isInvincible)
        {
            _isInvincible = false;
        }

        //Recover();
        healthBar.SetHealth(_currentHealth);
    }
    public void TakeDamage(float dmg)
    {
        //if the player is not currently invincible, reduce health and start invincibility
        if (!_isInvincible)
        {
            _currentHealth -= dmg;
            
            _invincibilityTimer = invincibilityDuration;
            _isInvincible = true;
            
            if (_currentHealth <= 0)
            {
                Kill();
            }
        }
    }

    public void Kill()
    {
        Debug.Log("PLAYER IS DEAD");
        Destroy(gameObject);
    }

    void Recover()
    {
        if (_currentHealth < maxHealth)
        {
            // Make sure the player's health doesn't exceed their max health
            if (_currentHealth > maxHealth)
            {
                _currentHealth = maxHealth;
            }
        }
    }
}
