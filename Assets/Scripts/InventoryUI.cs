using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
//using UnityEngine.UIElements; // Only needed if mixing UI Toolkit, otherwise remove

public class InventoryUI : MonoBehaviour
{
    [Header("References")]
    public InventoryManager inventoryManager;
    public GameObject inventoryPanel;
    
    // CRITICAL: Drag the "Content" object (inside Viewport) here, NOT the Scroll View
    public Transform slotsContainer; 
    public Image slotPrefab; 

    [Header("Settings")]
    public int backpackRows = 4;
    public int backpackCols = 6;

    private bool isOpen = false;
    private InputSystem_Actions inputActions;

    void Awake()
    {
        inputActions = new InputSystem_Actions();
        inventoryPanel.SetActive(false);
    }

    void OnEnable() => inputActions.Enable();
    void OnDisable() => inputActions.Disable();

    void Update()
    {
        if (inputActions.Player.ToggleInventory.WasPressedThisFrame() || inputActions.Player.ToggleInventory.triggered)
        {
            ToggleInventory();
        }
    }

    void ToggleInventory()
    {
        isOpen = !isOpen;
        inventoryPanel.SetActive(isOpen);

        // Optional: Pause Game
        // Time.timeScale = isOpen ? 0 : 1; 

        if (isOpen)
        {
            RefreshUI();
        }
    }

    public void RefreshUI()
    {
        // 1. Clear existing slots
        foreach (Transform child in slotsContainer)
        {
            Destroy(child.gameObject);
        }

        // 2. Calculate Total Slots Needed
        // We generate enough slots for Hotbar + Full Backpack Grid
        int totalSlots = inventoryManager.hotbarSize + (backpackRows * backpackCols);
        
        // 3. Generate Slots
        for (int i = 0; i < totalSlots; i++)
        {
            Image newSlot = Instantiate(slotPrefab, slotsContainer);
            newSlot.name = "Slot_" + i;

            ItemData item = null;

            // Logic: Fill Hotbar first, then Backpack
            if (i < inventoryManager.hotbarSize)
            {
                // Hotbar Slot
                if (i < inventoryManager.hotbar.Length)
                    item = inventoryManager.hotbar[i];
            }
            else
            {
                // Backpack Slot
                // Calculate index in the main inventory list
                // Note: This assumes inventory list contains ONLY backpack items if hotbar is full,
                // OR you need to skip hotbar items in the list. 
                // Simplest approach for now: Map list index directly if you stored overflow there.
                
                int backpackIndex = i - inventoryManager.hotbarSize;
                
                // Safety check: Ensure we don't crash if inventory is smaller than grid
                if (backpackIndex < inventoryManager.inventory.Count)
                {
                    // If your InventoryManager stores ALL items in 'inventory' list, 
                    // you need logic to skip the ones already in hotbar.
                    // For this example, assuming 'inventory' holds the overflow items only:
                    item = inventoryManager.inventory[backpackIndex];
                }
            }

            // 4. Update Visuals
            if (item != null && item.icon != null)
            {
                newSlot.sprite = item.icon;
                newSlot.color = Color.white;
                newSlot.enabled = true;
            }
            else
            {
                newSlot.sprite = null;
                newSlot.color = new Color(0.2f, 0.2f, 0.2f, 1f); // Dark empty slot
                // Optional: newSlot.enabled = false;
            }
        }

        // 5. CRITICAL: Force Layout Rebuild
        // This tells the Scroll View to recalculate its content height immediately
        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)slotsContainer);
    }
}   