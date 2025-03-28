using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ItemSO : ScriptableObject {
    public Item itemPrefab;
    public Type itemType;
    public Sprite sprite;
    public string itemName;
    public bool isStackable;
    public int stackLimit;
    public List<ModifyStat> statsToModify = new List<ModifyStat>();

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
    
    [System.Serializable]
    public struct ModifyStat {
        public StatToChange stat;
        public int value;
    }
}
