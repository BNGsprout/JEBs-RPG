using UnityEngine;

[CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue/Conversation")]
public class Dialogue : ScriptableObject
{
    [TextArea(3, 10)]
    public string[] lines; // Array of dialogue lines
    public string npcName; // Name of the speaker
}   