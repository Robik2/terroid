using Inventory;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player {
    public class UsingItems : MonoBehaviour {
        private float lastWeaponUse;
        private float lastHealingUse;
        [SerializeField] private float healingCooldown;

        private bool LMB;
        private bool canHeal;

        private int currentItemAnim;
        
        private void Start() {
            canHeal = true;
            currentItemAnim = 0;
        }

        private void Update() {
            if (LMB == true) {
                UseItem();
            }

            if (Time.time - lastHealingUse >= healingCooldown && canHeal == false) canHeal = true;
        }

        public void MouseInput(InputAction.CallbackContext context) {
            if (context.performed && InventoryManager.instance.CanUseItem()) {
                LMB = true;
            }

            if (context.canceled) LMB = false;
        }

        private void UseItem() {
            UIItem item = UIInput.instance.IsHoldingItem == true ? UIInput.instance.HeldItem : InventoryManager.instance.selectedSlot.containedItem;
            if (item == null) return;
            
            switch (item.itemSO) {
                case ItemWeapon weapon:
                    if (Time.time - lastWeaponUse < 1f / weapon.attackSpeed) return;
                    
                    weapon.UseItem(transform, AnimToPlay(weapon));
                    lastWeaponUse = Time.time;
                    break;
                
                case ItemConsumable consumable:
                    LMB = false;

                    if (consumable.isHealingItem == true) {
                        if (canHeal == false) return;

                        lastHealingUse = Time.time;
                        canHeal = false;
                        consumable.UseItem();
                        item.UpdateAmount(-1, false);
                    }
                    break;
            }
        }

        private int AnimToPlay(ItemWeapon item) {
            if (Time.time - lastWeaponUse > item.comboDuration || currentItemAnim == item.animCount) { // IF PLAYER DIDNT USE WEAPON QUICKLY ENOUGH OR IT REACHING LAST ANIMATION THEN SET IT BACK TO FIRST
                currentItemAnim = 1;
            } else {
                currentItemAnim++;
            }
                
            return currentItemAnim;
        }
    }
}