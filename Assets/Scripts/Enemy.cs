using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    [Header("Stats")]
    public float maxHealth = 50f;
    public float currentHealth;
    public float moveSpeed = 3f;
    public float damage = 10f;
    public float detectRange = 7f;
    public float attackRange = 1.2f;
    public float attackSpeed = 1f;
    private float timeSinceAttack = 0f;
    public bool hasAttacked = false;

    [Header("Loot")]
    public ItemData dropItem; // Optional: Drag an item here to drop on death

    [Header("UI")]
    public Slider healthSlider; // Drag your Slider object here
    public GameObject healthBarCanvas; // Drag the Canvas object here (to hide on death)

    private Transform player;
    private Rigidbody2D rb;
    private bool isDead = false;
    private PlayerStats playerStats;

    void Awake()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerStats = player.GetComponent<PlayerStats>();
        
        // Setup Slider limits
        healthSlider.maxValue = maxHealth;
        healthSlider.value = currentHealth;
    }

    void Update()
    {
        if (isDead || playerStats == null) return;

        float distance = Vector2.Distance(transform.position, player.position);
        
        // Chase Player
        if (distance <= detectRange && distance > attackRange)
        {
        //    Debug.Log("Placeholder");
        //    Vector2 direction = (player.position - transform.position).normalized;
        //    rb.MovePosition(rb.position + direction * moveSpeed * Time.fixedDeltaTime);
        }
        // Attack Player
        else if (distance <= attackRange)
        {
            AttackPlayer();
        }
    }

    void AttackPlayer()
    {
        if (isDead) return;

        timeSinceAttack += Time.deltaTime;
        if (timeSinceAttack >= attackSpeed)
        {
            hasAttacked = false;
        }
        
        if (hasAttacked == false)
        {
            hasAttacked = true;
            timeSinceAttack = 0f;
            //AttackCooldown();
            playerStats.ModifyHealth(-damage);
            Debug.Log("Enemy hit player for " + damage + " damage!");
        }
    }

    void AttackCooldown()
    {
        //timeSinceAttack += Time.deltaTime;
        //if (timeSinceAttack >= attackSpeed)
        //{
        //    hasAttacked = false;
        //}
    }

    public void TakeDamage(int amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        Debug.Log("Enemy took " + amount + " damage. Health: " + currentHealth);
        healthSlider.value = currentHealth;

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;
        Debug.Log("Enemy died!");

        // Drop loot
        if (dropItem != null)
        {
            // Instantiate item pickup at enemy position
            // You'll need a prefab with ItemPickup component
            //Instantiate(itemPrefab, transform.position, Quaternion.identity);
        }
         if (healthBarCanvas != null)
            healthBarCanvas.SetActive(false);

        Destroy(gameObject);
    }

    // Optional: Flash red when hit
    void OnDamageVisual()
    {
        // Add sprite flash or knockback here
    }
}   