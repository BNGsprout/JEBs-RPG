using UnityEngine;
using System;

public class PlayerStats : MonoBehaviour
{
    // Events to notify UI or other systems when stats change
    public event Action<float, float> OnHealthChanged;      // (current, max)
    public event Action<float, float> OnHungerChanged;      // (current, max)
    public event Action<float, float> OnManaChanged;        // (current, max)
    public event Action OnPlayerDeath;

    [Header("Base Stats")]
    public float maxHealth;
    public float health;
    public float maxHunger;
    public float hunger;
    public float maxMana;
    public float mana;
    public int reliance;
    public float relianceBar;
    public float speed;
    public int strength;
    public float defence;
    public float experience;
    public int level;
    public int intelligence;
    public float fireResistance;
    public float iceResistance;
    public float electricityResistance;
    public float poisonResistance;


    [Header("Current Stats")]
    [SerializeField] public float currentHealth;
    [SerializeField] public float currentHunger;
    [SerializeField] public float currentMana;

    [Header("Regen / Decay Rates")]
    public float hungerDecayRate = 2f; // Points per second
    public float healthRegenRate = 1f; // Points per second (only if not starving)
    public float manaRegenRate = 5f;   // Points per second

    private bool isAlive = true;

        void Awake() // Change from Start() to Awake()
    {
        var data = SaveManager.CurrentPlayer;
        if (data == null) return;

        maxHealth = data.maxHealth;
        health = data.health;
        maxMana = data.maxMana;
        mana = data.mana;
        hunger = data.hunger;
        maxHunger = data.maxHunger;
        reliance = data.reliance;
        relianceBar = data.relianceBar;
        speed = data.speed;
        strength = data.strength;
        defence = data.defence;
        experience = data.experience;
        level = data.level;
        intelligence = data.intelligence;
        fireResistance = data.fireResistance;
        iceResistance = data.iceResistance;
        electricityResistance = data.electricityResistance;
        poisonResistance = data.poisonResistance;

        // Initialize current stats to max immediately
        currentHealth = health;
        currentHunger = hunger;
        currentMana = mana;
        
        // Optional: Manually invoke events once so UI updates immediately on start
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        OnHungerChanged?.Invoke(currentHunger, maxHunger);
        OnManaChanged?.Invoke(currentMana, maxMana);
    }

    // You can remove the Start() method entirely if it's now empty   

    void Update()
    {
        if (!isAlive) return;

        // Apply Decay/Regen over time
        ModifyHunger(-hungerDecayRate * Time.deltaTime);

        // Starvation Logic: If hunger is 0, lose health
        if (currentHunger <= 0f)
        {
            ModifyHealth(-healthRegenRate * Time.deltaTime); // Reuse regen var as damage var when starving
        }
        else
        {
            // Only regen health if not starving
            if (currentHealth < maxHealth)
                ModifyHealth(healthRegenRate * Time.deltaTime);
        }

        // Mana Regen
        if (currentMana < maxMana)
            ModifyMana(manaRegenRate * Time.deltaTime);
    }

    // --- Public Methods to Modify Stats ---

    public void ModifyHealth(float amount)
    {
        float previous = currentHealth;
        currentHealth = Mathf.Clamp(currentHealth + amount, 0f, maxHealth);
        
        if (currentHealth != previous)
            OnHealthChanged?.Invoke(currentHealth, maxHealth);

        if (currentHealth <= 0f && isAlive)
            Die();
    }

    public void ModifyHunger(float amount)
    {
        float previous = currentHunger;
        currentHunger = Mathf.Clamp(currentHunger + amount, 0f, maxHunger);

        if (currentHunger != previous)
            OnHungerChanged?.Invoke(currentHunger, maxHunger);
    }

    public void ModifyMana(float amount)
    {
        float previous = currentMana;
        currentMana = Mathf.Clamp(currentMana + amount, 0f, maxMana);

        if (currentMana != previous)
            OnManaChanged?.Invoke(currentMana, maxMana);
    }

    public bool UseMana(float amount)
    {
        if (currentMana >= amount)
        {
            ModifyMana(-amount);
            return true;
        }
        return false; // Not enough mana
    }

    void Die()
    {
        isAlive = false;
        currentHealth = 0;
        OnPlayerDeath?.Invoke();
        Debug.Log("Player Died!");
        // TODO: Trigger respawn or game over screen
    }

    // Optional: Heal/Eat functions for items
    public void Heal(float amount) => ModifyHealth(amount);
    public void Eat(float amount) => ModifyHunger(amount);
}   