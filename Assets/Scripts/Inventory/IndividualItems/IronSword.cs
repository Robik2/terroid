using Inventory;
using ObjectPooling;
using Player;
using UnityEngine;

namespace Inventory {
    [CreateAssetMenu(fileName = "IronSword", menuName = "Scriptable Objects/Weapons/Iron Sword")]
    public class IronSword : ItemWeapon {
        [SerializeField] private GameObject swordSlash;
        
        public override void UseItem(Transform pivot) {
            GameObject obj = ObjectPoolingManager.SpawnObject(swordSlash, pivot);
            obj.transform.rotation = PlayerController.instance.ToMouseRotation;
        }
    }
}