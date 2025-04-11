using ObjectPooling;
using Player;
using UnityEngine;

namespace Inventory {
    [CreateAssetMenu(fileName = "IronSword", menuName = "Scriptable Objects/Weapons/Iron Sword")]
    public class IronSword : ItemWeapon {
        public GameObject swordSlash;
        
        public override void UseItem(Transform pivot, int currentAnim) {
            GameObject obj = ObjectPoolingManager.SpawnObject(swordSlash, pivot);
            obj.GetComponent<ApplyHit>().value = damageValue;
            obj.GetComponent<Animator>().SetInteger("currentAnim", currentAnim);
            obj.transform.rotation = PlayerController.instance.ToMouseRotation;
        }
    }
}