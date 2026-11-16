using UnityEngine;
using UnityEngine.UI;

public class UIHotbar : MonoBehaviour
{
    [Header("References")]
    public InventoryManager inventoryManager;
    public Image[] slotImages;      // Drag Slot1, Slot2, Slot3, Slot4 here
    public Image selector;          // Drag the Selector highlight here
    public Color emptySlotColor = new Color(0.2f, 0.2f, 0.2f, 1f);
    public Color filledSlotColor = Color.white;

    void Start()
    {
        if (inventoryManager == null)
            inventoryManager = FindAnyObjectByType<InventoryManager>();
            
        UpdateHotbarUI();
    }

    void Update()
    {
        // Update every frame to catch slot switches and item changes
        UpdateHotbarUI();
    }

    void UpdateHotbarUI()
    {
        if (inventoryManager == null || slotImages == null) return;

        // 1. Update Slot Icons
        for (int i = 0; i < slotImages.Length; i++)
        {
            // Ensure we don't go out of bounds if hotbarSize changes
            if (i < inventoryManager.hotbar.Length)
            {
                ItemData item = inventoryManager.hotbar[i];

                if (item != null && item.icon != null)
                {
                    slotImages[i].sprite = item.icon;
                    slotImages[i].color = filledSlotColor;
                    slotImages[i].enabled = true;
                }
                else
                {
                    slotImages[i].sprite = null;
                    slotImages[i].color = emptySlotColor;
                    // Optional: Keep enabled but dark, or disable entirely
                    // slotImages[i].enabled = false; 
                }
            }
        }

        // 2. Move Selector Highlight
        if (selector != null && inventoryManager.currentSlot >= 0 && inventoryManager.currentSlot < slotImages.Length)
        {
            selector.transform.position = slotImages[inventoryManager.currentSlot].transform.position;
            selector.enabled = true;
        }
        else if (selector != null)
        {
            selector.enabled = false;
        }
    }
}   