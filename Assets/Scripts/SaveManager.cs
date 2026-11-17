using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using System.Collections.Generic;

public enum PlayerClass
{
    Knight,
    Mage,
    Rogue
}

[System.Serializable]
public class WorldData
{
    public string worldName;
    public int seed;
}

[System.Serializable]
public class PlayerData
{
    public string playerName;
    public string playerID;
    public PlayerClass playerClass;
    public float maxHealth;
    public float health;
    public float maxMana;
    public float mana;
    public float maxHunger;
    public float hunger;
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
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button[] classButtons;
    [SerializeField] private Button confirmClassButton;

    private const int MaxSlots = 4;
    private string savesRoot;
    private int selectedSlot = -1;
    private string pendingName = "";
    private PlayerClass pendingClass;

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

        confirmButton.onClick.AddListener(OnConfirmClicked);
        confirmClassButton.onClick.AddListener(OnConfirmClassClicked);

        for (int i = 0; i < classButtons.Length; i++)
        {
            int cls = i;
            classButtons[i].onClick.AddListener(() => OnClassClicked(cls));
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

    // --- Button handlers ---

    private void OnSlotClicked(int slot)
    {
        if (HasSave(slot))
        {
            var (world, player) = Load(slot);
            Debug.Log($"Loading {world.worldName} ({player.playerName})");
            // ... load scene, apply data, etc.
        }
        else
        {
            selectedSlot = slot;
            worldNameField.text = "";
            // your menu manager shows the name panel
        }
    }

    private void OnConfirmClicked()
    {
        string name = string.IsNullOrWhiteSpace(worldNameField.text)
            ? "New World"
            : worldNameField.text.Trim();

        name = string.Join("_", name.Split(Path.GetInvalidFileNameChars()));

        pendingName = name;
        // your menu manager shows the class selection screen
    }

    private void OnClassClicked(int classIndex)
{
    pendingClass = (PlayerClass)classIndex;
    // your menu manager shows a preview/confirm screen
}

    private void OnConfirmClassClicked()
{
    var world = new WorldData { worldName = pendingName, seed = Random.Range(0, 99999) };
    var player = CreatePlayerData(pendingName, pendingClass);

    Save(selectedSlot, world, player);
    // your menu manager handles the rest
}

    // --- Class factory ---

    private PlayerData CreatePlayerData(string name, PlayerClass cls)
    {
        return cls switch
        {
            PlayerClass.Knight => new PlayerData
            {
                playerName = name,
                playerID = System.Guid.NewGuid().ToString(),
                playerClass = cls,
                maxHealth = 100f,
                health = 100f,
                maxMana = 50f,
                mana = 50f,
                maxHunger = 100f,
                hunger = 100f,
                reliance = 3,
                relianceBar = 0f,
                speed = 9f,
                strength = 3,
                defence = 3f,
                experience = 0f,
                level = 0,
                intelligence = 3,
                fireResistance = 0f,
                iceResistance = 0f,
                electricityResistance = 0f,
                poisonResistance = 1f
            },
            PlayerClass.Mage => new PlayerData
            {
                playerName = name,
                playerID = System.Guid.NewGuid().ToString(),
                playerClass = cls,
                health = 100f,
                mana = 150f,
                hunger = 100,
                reliance = 50,
                relianceBar = 100f,
                speed = 7f,
                strength = 5,
                defence = 8f,
                experience = 0f,
                level = 1,
                intelligence = 15,
                fireResistance = 0.15f,
                iceResistance = 0.1f,
                electricityResistance = 0.05f,
                poisonResistance = 0f
            },
            PlayerClass.Rogue => new PlayerData
            {
                playerName = name,
                playerID = System.Guid.NewGuid().ToString(),
                playerClass = cls,
                health = 150f,
                mana = 60f,
                hunger = 100,
                reliance = 50,
                relianceBar = 100f,
                speed = 12f,
                strength = 10,
                defence = 12f,
                experience = 0f,
                level = 1,
                intelligence = 8,
                fireResistance = 0f,
                iceResistance = 0f,
                electricityResistance = 0.1f,
                poisonResistance = 0.15f
            },
            _ => throw new System.ArgumentException($"Unknown class: {cls}")
        };
    }

    // --- UI ---

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