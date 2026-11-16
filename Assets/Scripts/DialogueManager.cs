using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro; // Recommended for better text handling

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    [Header("UI References")]
    public GameObject dialoguePanel;
    public TextMeshProUGUI npcNameText;
    public TextMeshProUGUI dialogueText;
    
    private Dialogue currentDialogue;
    private int currentLineIndex = 0;
    private bool isTyping = false;
    bool dialogueOpen;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        
        dialoguePanel.SetActive(false);
        dialogueOpen = false;
    }

    public void OnInteract()
    {
        if (dialogueOpen)
        {
            dialoguePanel.SetActive(false);
        }
    }

    public void StartDialogue(Dialogue dialogue)
    {
        if (dialogueOpen) dialoguePanel.SetActive(false);

        dialogueOpen = true;
        currentDialogue = dialogue;
        currentLineIndex = 0;
        dialoguePanel.SetActive(true);
        ShowLine();
        
        // Optional: Disable player movement here
        // PlayerController.Instance.SetMovable(false);
    }

    

    public void AdvanceDialogue()
{
    // If we are at the end of the lines, close the dialogue immediately
    if (currentLineIndex >= currentDialogue.lines.Length)
    {
        EndDialogue();
        return;
    }

    // Otherwise, show the next line
    ShowLine();
}

// Update ShowLine to simply display text without changing state logic
void ShowLine()
{
    // Safety check
    if (currentLineIndex >= currentDialogue.lines.Length) return;

    npcNameText.text = currentDialogue.npcName;
    dialogueText.text = currentDialogue.lines[currentLineIndex];
    
    // Increment index ONLY after showing the line
    currentLineIndex++;
}   
    void EndDialogue()
    {
        dialogueOpen = false;
        dialoguePanel.SetActive(false);
        // Optional: Re-enable player movement
        // PlayerController.Instance.SetMovable(true);
        currentDialogue = null;
    }

    // In DialogueManager.cs

}   