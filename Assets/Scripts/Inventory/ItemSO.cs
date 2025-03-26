using UnityEngine;

[CreateAssetMenu]
public class ItemSO : ScriptableObject {
    public Type itemType;
    public string itemName;
    public StatToChange statToChange;
    public int value;

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

    public bool UseItem() {
        Debug.Log($"{statToChange} changed by {value}");
        return true; // IN CASE I WOULD LIKE TO LIMIT THIS SOMEHOW
    }
}
