using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// This class is a bridge to the Battle (UI)
public class BattleVisuals : MonoBehaviour
{
    [SerializeField] private Slider healthBar;
    [SerializeField] private TextMeshProUGUI levelText;
    
    private int currrHealth;
    private int maxHealth;
    private int level;
    
    
    private const string LEVEL_ABB = "Level: "; //ABBREVIATION
    void Start()
    {
        SetStartingValues(10,10,5);
    }

    public void SetStartingValues(int currHealth, int maxHealth, int level)
    {
        this.currrHealth = currHealth;
        this.maxHealth = maxHealth;
        this.level = level;
        levelText.text = LEVEL_ABB + this.level.ToString();
        UpdateHealthBar();
    }

    public void ChangeHealth(int currHealth)
    {
        this.currrHealth = currHealth;
        // if 0 = Death
    }

    // Update HealthBar
    public void UpdateHealthBar()
    {
        healthBar.maxValue = maxHealth;
        healthBar.value = currrHealth;
    }
}
