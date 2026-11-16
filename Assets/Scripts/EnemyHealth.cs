using UnityEngine;
using UnityEngine.UI; // Required for Slider

public class EnemyHealth : MonoBehaviour
{
    [Header("Stats")]
    public float maxHealth = 100f;
    private float currentHealth;

    [Header("UI")]
    public Slider healthSlider; // Drag your Slider object here
    public GameObject healthBarCanvas; // Drag the Canvas object here (to hide on death)

    void Start()
    {
        currentHealth = maxHealth;
        
        // Setup Slider limits
        healthSlider.maxValue = maxHealth;
        healthSlider.value = currentHealth;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        // Directly set the slider value (no math needed!)
        healthSlider.value = currentHealth;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        // Stop movement
        UnityEngine.AI.NavMeshAgent agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        if (agent != null) agent.enabled = false;

        // Hide the entire canvas
        if (healthBarCanvas != null)
            healthBarCanvas.SetActive(false);
            
        // Destroy after delay
        Destroy(gameObject, 2f); 
    }
}