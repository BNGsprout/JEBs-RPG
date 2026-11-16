using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq; // Required for OrderBy and Take

public class InventoryManager : MonoBehaviour
{
    [Header("Singleton")]
    public static InventoryManager Instance;

    [Header("Settings")]
    public int hotbarSize = 4;
    public float interactRange = 1.5f;
    public float moveSpeed = 5f;
    public int maxTargets = 2;
    //[SerializeField] StatBarUI ;

    [Header("State")]
    public System.Collections.Generic.List<ItemData> inventory = new System.Collections.Generic.List<ItemData>();
    public ItemData[] hotbar;
    public int currentSlot = 0;
    
    public ItemData CurrentItem => (hotbar != null && currentSlot < hotbarSize) ? hotbar[currentSlot] : null;

    private Rigidbody2D rb;
    private Vector2 movementInput;
    private InputSystem_Actions inputActions;
    private ItemPickup nearbyItem;

    void Awake()
    {
        // Singleton Setup
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        rb = GetComponent<Rigidbody2D>();
        hotbar = new ItemData[hotbarSize];
        inputActions = new InputSystem_Actions();
    }

    void OnEnable()
    {
        inputActions.Enable();
    }

    void OnDisable()
    {
        inputActions.Disable();
    }

    void Update()
    {
        HandleInput();
        CheckNearbyItems();
    }

    void FixedUpdate()
    {
        HandleMovement();
    }

    void HandleInput()
    {
        // 1. Movement
        movementInput = inputActions.Player.Move.ReadValue<Vector2>();

        // 2. Hotbar Switching (Scroll Wheel / Bumpers)
        // We use .triggered for reliable single-fire detection
        if (inputActions.Player.NextSlot.WasPressedThisFrame() || inputActions.Player.NextSlot.triggered)
        {
            ChangeSlot(1);
        }
        else if (inputActions.Player.PrevSlot.WasPressedThisFrame() || inputActions.Player.PrevSlot.triggered)
        {
            ChangeSlot(-1);
        }

        // 3. Pickup (E / A-Cross)
        if (inputActions.Player.Pickup.WasPressedThisFrame() || inputActions.Player.Pickup.triggered)
        {
            if (nearbyItem != null)
            {
                nearbyItem.TryPickup();
            }
        }

        // 4. Attack/Use (Left Click / X-Square)
        if (inputActions.Player.Attack.WasPressedThisFrame() || inputActions.Player.Attack.triggered)
        {
            UseItem();
        }
    }

    void ChangeSlot(int direction)
    {
        currentSlot += direction;
        
        // Loop around the hotbar
        if (currentSlot < 0) currentSlot = hotbarSize - 1;
        if (currentSlot >= hotbarSize) currentSlot = 0;

        EquipItem();
    }

    void CheckNearbyItems()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, interactRange);
        nearbyItem = null;

        foreach (var col in colliders)
        {
            ItemPickup item = col.GetComponent<ItemPickup>();
            if (item != null)
            {
                nearbyItem = item;
                break;
            }
        }
    }

    void HandleMovement()
    {
        if (rb != null)
        {
            rb.MovePosition(rb.position + movementInput * moveSpeed * Time.fixedDeltaTime);
        }
    }

    // --- Inventory Functions ---

        public void AddItem(ItemData item)
    {
        if (item == null) return;

        // 1. Try to add to Hotbar
        bool addedToHotbar = false;
        for (int i = 0; i < hotbarSize; i++)
        {
            if (hotbar[i] == null)
            {
                hotbar[i] = item;
                currentSlot = i;
                addedToHotbar = true;
                Debug.Log("Equipped: " + item.itemName);
                break;
            }
        }

        // 2. If Hotbar full, add to Backpack (Inventory List)
        if (!addedToHotbar)
        {
            inventory.Add(item);
            Debug.Log("Hotbar full! Stored " + item.itemName + " in backpack.");
        }
        
        // Notify UI to refresh if open
        FindAnyObjectByType<InventoryUI>().RefreshUI();
    }   

    void EquipItem()
    {
        if (CurrentItem != null)
            Debug.Log("Equipped: " + CurrentItem.itemName + " (Slot " + (currentSlot + 1) + ")");
        else
            Debug.Log("Slot " + (currentSlot + 1) + " is empty");
            
        // TODO: Update Player Sprite/Animation here
    }

    void UseItem()
    {
        if (CurrentItem == null)
        {
            // Optional: Play "empty swing" animation
            return;
        }

        if (CurrentItem.itemType == ItemData.ItemType.Weapon)
        {
            PerformAttack(CurrentItem);
        }
        else if (CurrentItem.itemType == ItemData.ItemType.Consumable)
        {
            //StatBarUI
            //Change hunger bar when item eaten
            Debug.Log("Used: " + CurrentItem.itemName);
            hotbar[currentSlot] = null; 
            EquipItem();
        }
    }
    
    void PerformAttack(ItemData weapon)
    {
        Debug.Log("Attacking with " + weapon.itemName + "!");

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, weapon.attackRange);
        // --- REPLACE YOUR FOREACH LOOP WITH THIS ---

// 1. Sort all hits by distance and take only the closest 2
var closestHits = hits
    .Where(hit => hit.CompareTag("Enemy")) // Keep only enemies
    .OrderBy(hit => Vector2.Distance(transform.position, hit.transform.position)) // Sort by distance
    .Take(maxTargets); // Limit to 2 targets

// 2. Loop only through those 2 enemies
foreach (var hit in closestHits)
{
    Debug.Log("Hit Enemy: " + hit.name);
    
    // Apply damage directly to 'hit', do NOT search for the enemy again
    Enemy enemyInstance = hit.GetComponent<Enemy>();
    
    if (enemyInstance != null)
    {
        enemyInstance.TakeDamage(weapon.damage);
    }
}

// --- END REPLACEMENT ---   
    }
}