using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    public ItemType itemType; // Enum: Weapon, Consumable, Material
    
    // Item Stats
    public int damage;
    public float attackRange;
    public float nutrition;
    public float manaRegen;
    public float healing;
    public float brightness;
    //public GameObject attackPrefab; // Optional: Projectile or VFX


    public enum ItemType { Weapon, Consumable, Item, Other }
}   