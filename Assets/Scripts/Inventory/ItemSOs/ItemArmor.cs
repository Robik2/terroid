using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewArmor", menuName = "Items/Item Armor SO")]
public class ItemArmor : ItemSO {
    public ArmorType armorType;
    public List<ModifyStat> statsToModify = new();
    
    public enum ArmorType {
        head,
        chest,
        legs
    }
    
    public enum StatToChange {
        health,
        defense,
        [InspectorName("Melee Damage (%)")] meleeDamage,
        [InspectorName("Magic Damage (%)")] magicDamage,
        [InspectorName("Range Damage (%)")] rangeDamage,
        critChance,
        [InspectorName("Attack Speed (%)")] attackSpeed
    };
    
    [System.Serializable]
    public struct ModifyStat {
        public StatToChange stat;
        public int value;
    }
}
