using UnityEngine;

[CreateAssetMenu]
public class ItemSO : ScriptableObject {
    public Item itemPrefab;
    public Type itemType;
    public Sprite sprite;
    public string itemName;
    public StatToChange statToChange;
    public int value;
    public bool isStackable;
    public int stackLimit;

    public enum Type {
        consumable,
        weapon,
        armor
    }
    
    public enum StatToChange {
        none,
        health,
        mana,
        magicPower
    };
}
