using UnityEngine;
using UnityEngine.UI;

// 1. Move the Enum OUTSIDE the class (or at the very top, before any [Header] attributes)
public enum StatType { Health, Hunger, Mana }

[RequireComponent(typeof(Slider))]
public class StatBarUI : MonoBehaviour
{
    // 2. Now the [Header] attributes are valid on fields
    [Header("References")]
    public PlayerStats playerStats;
    
    [Header("Stat Type")]
    public StatType statType; // This field uses the enum defined above

    private Slider slider;

    void Awake()
    {
        slider = GetComponent<Slider>();
        
        if (playerStats == null)
            playerStats = FindAnyObjectByType<PlayerStats>();

        if (playerStats == null)
        {
            Debug.LogError("PlayerStats not found!");
            return;
        }

        // Subscribe to events based on type
        switch (statType)
        {
            case StatType.Health:
                playerStats.OnHealthChanged += UpdateBar;
                break;
            case StatType.Hunger:
                playerStats.OnHungerChanged += UpdateBar;
                break;
            case StatType.Mana:
                playerStats.OnManaChanged += UpdateBar;
                break;
        }

        InitializeBar();
    }

    void OnDestroy()
    {
        if (playerStats != null)
        {
            playerStats.OnHealthChanged -= UpdateBar;
            playerStats.OnHungerChanged -= UpdateBar;
            playerStats.OnManaChanged -= UpdateBar;
        }
    }

    void InitializeBar()
    {
        switch (statType)
        {
            case StatType.Health: UpdateBar(playerStats.currentHealth, playerStats.maxHealth); break;
            case StatType.Hunger: UpdateBar(playerStats.currentHunger, playerStats.maxHunger); break;
            case StatType.Mana:   UpdateBar(playerStats.currentMana, playerStats.maxMana); break;
        }
    }

    void UpdateBar(float current, float max)
    {
        slider.maxValue = max;
        slider.value = current;
    }
}   