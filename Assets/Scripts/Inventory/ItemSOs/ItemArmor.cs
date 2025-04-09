using System.Collections.Generic;
using UnityEngine;
using Inventory;

namespace Inventory {
    [CreateAssetMenu(fileName = "NewArmor", menuName = "Scriptable Objects/Item Armor SO")]
    public class ItemArmor : ItemSO {
        public ArmorType armorType;
        public List<ModifyStat> statsToModify = new();

        public enum ArmorType {
            head,
            chest,
            legs
        }

        [System.Serializable]
        public struct ModifyStat {
            public StatToChange stat;
            public int value;
        }
    }
}