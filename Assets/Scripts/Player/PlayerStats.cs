using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
    //Current stats
    public float maxHealth;
    public float defaultDamage;
    private float _currentHealth;
    [HideInInspector]
    public float currentDamage;
    
    [Header("UI Elements")]
    public Image[] hearts;
    public Sprite fullHeart;
    public Sprite emptyHeart;

    //I-Frames
    [Header("I-Frames")]
    public float invincibilityDuration;
    float _invincibilityTimer;
    private bool _isInvincible;

    private GameManager _gameManager;
    
    void Start()
    {
        _currentHealth = maxHealth;
        currentDamage = defaultDamage;
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
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

        for (int i = 0; i < hearts.Length; i++)
        {
            if (i < _currentHealth)
                hearts[i].sprite = fullHeart;
            else
                hearts[i].sprite = emptyHeart;
            if (i < maxHealth)
                hearts[i].enabled = true;
            else
                hearts[i].enabled = false;
        }
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
                _gameManager.PlayerDeath();
            }
        }
    }

    public void ResetStats()
    {
        _currentHealth = maxHealth;
    }
}
