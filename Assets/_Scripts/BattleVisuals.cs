using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// This class is a bridge to the Battle (UI)
public class BattleVisuals : MonoBehaviour
{
    [SerializeField] private Slider healthBar;
    [SerializeField] private TextMeshProUGUI levelText;
    
    private int currHealth;
    private int maxHealth;
    private int level;
    
    [SerializeField] private Animator anim;
    
    private const string LEVEL_ABB = "Lvl: ";

    // Animation
    private const string IS_ATTACK_PARAM = "IsAttack";
    private const string IS_HIT_PARAM = "IsHit";
    private const string IS_DEAD_PARAM = "IsDead";
    void Start()
    {
        //anim = gameObject.GetComponent<Animator>();
    }

    public void SetStartingValues(int currHealth, int maxHealth, int level)
    {
        this.currHealth = currHealth;
        this.maxHealth = maxHealth;
        this.level = level;
        levelText.text = LEVEL_ABB + this.level.ToString();
        UpdateHealthBar();
    }

    public void ChangeHealth(int currHealth)
    {
        this.currHealth = currHealth;

        if (currHealth <= 0)
        {
            PlayDeathAnimation();
            Destroy(gameObject,1f);
        }
        UpdateHealthBar();
    }

    // Update HealthBar
    public void UpdateHealthBar()
    {
        healthBar.maxValue = maxHealth;
        healthBar.value = currHealth;
    }

    public void PlayAttackAnimation()
    {
        anim.SetTrigger(IS_ATTACK_PARAM);
    }
    
    public void PlayHitAnimation()
    {
        anim.SetTrigger(IS_HIT_PARAM);
    }
    
    public void PlayDeathAnimation()
    {
        anim.SetTrigger(IS_DEAD_PARAM);
    }
}
