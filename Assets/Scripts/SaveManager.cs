using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections.Generic;
using TMPro;

[System.Serializable]
public class WorldData
{
    public string worldName;
    public float seed;
}

[System.Serializable]
public class PlayerData
{
    public string playerName;
    public string playerID;
    public float health;
    public float mana;
    public int hunger;
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

}

public class SaveManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Button[] slotButtons;
    [SerializeField] private Text[] slotLabels;
    [SerializeField] private TMP_InputField worldNameField;

    private const int MaxSlots = 4;
    private string savesRoot;

    void Awake()
    {
        savesRoot = Path.Combine(Application.persistentDataPath, "Saves");
        if (!Directory.Exists(savesRoot))
            Directory.CreateDirectory(savesRoot);

        for (int i = 0; i < slotButtons.Length; i++)
        {
            int slot = i;
            slotButtons[i].onClick.AddListener(() => OnSlotClicked(slot));
        }
    }

    void Start()
    {
        RefreshLabels();
    }

    // --- Path helpers ---

    private string GetSlotDir(int slot)
    {
        return Path.Combine(savesRoot, $"slot_{slot}");
    }

    private string GetWorldPath(int slot) => Path.Combine(GetSlotDir(slot), "world.json");
    private string GetPlayerPath(int slot) => Path.Combine(GetSlotDir(slot), "player.json");

    // --- Slot detection ---

    public bool HasSave(int slot)
    {
        return File.Exists(GetWorldPath(slot));
    }

    public int GetSaveCount()
    {
        int count = 0;
        for (int i = 0; i < MaxSlots; i++)
            if (HasSave(i)) count++;
        return count;
    }

    public List<int> GetAvailableSlots()
    {
        var slots = new List<int>();
        for (int i = 0; i < MaxSlots; i++)
            if (!HasSave(i)) slots.Add(i);
        return slots;
    }

    // --- Save ---

    public void Save(int slot, WorldData world, PlayerData player)
    {
        string dir = GetSlotDir(slot);
        if (!Directory.Exists(dir))
            Directory.CreateDirectory(dir);

        File.WriteAllText(GetWorldPath(slot), JsonUtility.ToJson(world));
        File.WriteAllText(GetPlayerPath(slot), JsonUtility.ToJson(player));

        RefreshLabels();
    }

    // --- Load ---

    public (WorldData world, PlayerData player) Load(int slot)
    {
        if (!HasSave(slot))
            throw new System.InvalidOperationException($"No save found in slot {slot}");

        string worldJson = File.ReadAllText(GetWorldPath(slot));
        string playerJson = File.ReadAllText(GetPlayerPath(slot));

        return (
            JsonUtility.FromJson<WorldData>(worldJson),
            JsonUtility.FromJson<PlayerData>(playerJson)
        );
    }

    // --- Delete ---

    public void DeleteSave(int slot)
    {
        string dir = GetSlotDir(slot);
        if (Directory.Exists(dir))
            Directory.Delete(dir, true);

        RefreshLabels();
    }

    // --- UI ---

    private void OnSlotClicked(int slot)
{
    if (HasSave(slot))
    {
        var (world, player) = Load(slot);
        Debug.Log($"Loading {world.worldName} ({player.playerName})");
    }
    else
    {
        string name = string.IsNullOrWhiteSpace(worldNameField.text) 
            ? "New World" 
            : worldNameField.text.Trim();

        name = string.Join("_", name.Split(Path.GetInvalidFileNameChars())); // ← here

        var world = new WorldData { worldName = name, seed = Random.Range(0, 99999) };
        var player = new PlayerData { playerName = "Player", health = 100f, level = 0 };
        Save(slot, world, player);
    }
}   

    private void RefreshLabels()
    {
        if (slotLabels == null) return;

        for (int i = 0; i < MaxSlots && i < slotLabels.Length; i++)
        {
            if (slotLabels[i] == null) continue;

            if (HasSave(i))
            {
                var (world, _) = Load(i);
                slotLabels[i].text = $"Slot {i + 1}: {world.worldName}";
            }
            else
            {
                slotLabels[i].text = $"Slot {i + 1}: Empty";
            }
        }
    }
}   