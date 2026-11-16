using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public ItemData itemData;
    private bool playerNearby = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = true;
            Debug.Log("Player entered trigger: " + gameObject.name); // Debug
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = false;
            Debug.Log("Player exited trigger: " + gameObject.name); // Debug
        }
    }

    public void TryPickup()
    {
        // This check is vital!
        if (playerNearby) 
        {
            Debug.Log("Picking up: " + itemData.itemName);
            if (InventoryManager.Instance != null)
            {
                InventoryManager.Instance.AddItem(itemData);
                Destroy(gameObject);
            }
            else
            {
                Debug.LogError("InventoryManager Instance is NULL!");
            }
        }
        else
        {
            Debug.LogWarning("Tried to pickup, but player is not nearby.");
        }
    }
}   