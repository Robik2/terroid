using System.Collections.Generic;
using UnityEngine;
using Inventory;

namespace Inventory {
    [CreateAssetMenu(fileName = "NewArmor", menuName = "Scriptable Objects/Item Armor SO")]
    public class ItemArmor : ItemSO {
        public ArmorType armorType;
        public List<ModifyStat> statsToModify = new();

        public enum ArmorType {
            Head,
            Chest,
            Legs
        }

        [System.Serializable]
        public struct ModifyStat {
            public StatToChange stat;
            public int value;
        }
    }
}