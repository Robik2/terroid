using Sirenix.OdinInspector;

namespace Inventory {
    public class ItemWeapon : ItemSO {
        public DamageType damageType;
        public int damageValue;
        [LabelText("Crit Chance (0-100)")] public int critChance;
        public float attackSpeed;

        [HideIf("damageType", DamageType.melee)]
        public int range;
        
        [ShowIf("damageType", DamageType.melee)]
        public int animCount;
        
        [ShowIf("damageType", DamageType.melee)]
        public float comboDuration;

        public enum DamageType {
            melee,
            magic,
            range
        }
    }
}