using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Collider2D))]
public class NPCInteract : MonoBehaviour
{
    [Header("Settings")]
    public Dialogue dialogue;
    public float interactionRange = 1.5f;
    
    [Header("Input")]
    public PlayerInput playerInput; // Assign the Player GameObject with PlayerInput component
    private InputAction interactAction;
    
    private bool playerInRange = false;
    private GameObject player;

    void OnEnable()
    {
        // Find the "Interact" action from the PlayerInput component
        if (playerInput != null)
        {
            interactAction = playerInput.actions.FindAction("Interact");
            if (interactAction != null)
            {
                interactAction.performed += OnInteract;
            }
        }
    }

    void OnDisable()
    {
        if (interactAction != null)
        {
            interactAction.performed -= OnInteract;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            player = other.gameObject;
            // Optional: Show "Press E to talk" UI hint here
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            player = null;
            // Optional: Hide UI hint
        }
    }

    void OnInteract(InputAction.CallbackContext context)
    {
        // Only trigger if player is in range and not already in a conversation
        if (playerInRange && DialogueManager.Instance != null)
        {
            // Ensure the player pressing the button is the one in range
            // This prevents multiple NPCs triggering at once if ranges overlap
            if (context.performed)
            {
                DialogueManager.Instance.StartDialogue(dialogue);
            }
        }
    }
}   