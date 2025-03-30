using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "NewWeapon", menuName = "Scriptable Objects/Items/Item Weapon SO")]
public class ItemWeapon : ItemSO {
    public DamageType damageType;
    public int damageValue;
    public int critChance;
    public float attackSpeed;

    [HideIf("damageType", DamageType.melee)]
    public int range;

    public enum DamageType {
        melee,
        magic,
        range
    }
}
