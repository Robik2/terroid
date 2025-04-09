using Sirenix.OdinInspector;
using UnityEngine;
using Inventory;

namespace Inventory {
    public class ItemWeapon : ItemSO {
        public DamageType damageType;
        public int damageValue;
        [LabelText("Crit Chance (0-100)")] public int critChance;
        public float attackSpeed;

        [HideIf("damageType", DamageType.melee)]
        public int range;

        public enum DamageType {
            melee,
            magic,
            range
        }
    }
}