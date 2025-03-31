using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Inventory {
    public class UIItem : MonoBehaviour {
        [SerializeField] public ItemSO itemSO;
        public TMP_Text amountText;
        [ReadOnly] public int amount;
        [ReadOnly] public bool isFull;
        public ItemSlot slot;
        public bool isDividedByRMB;
        private bool isHeld;

        private void Update() {
            if (isHeld == true) { transform.position = Input.mousePosition; }
        }

        public int AddItem(int amountNew) {
            amount += amountNew;
            if (amount >= itemSO.stackLimit) {
                amountText.text = itemSO.stackLimit.ToString();
                isFull = true;

                int extraItems = amount - itemSO.stackLimit;
                amount = itemSO.stackLimit;
                return extraItems;
            }

            amountText.text = amount.ToString();
            return 0;
        }

        public void ToggleHold() {
            isHeld = !isHeld;
            GetComponent<Image>().raycastTarget = !isHeld;
        }

        public void UseItem() {
            switch (itemSO) {
                case ItemConsumable item: // LATER MAKE IT ACTUALLY AFFECT THOSE STATS
                    ConsumeItem();
                    break;

                // case ItemSO.Type.weapon: // LATER MAKE IT SO THAT THE WEAPON FIRE/ATTACK
                //     Debug.Log("This is a weapon: " + itemSO.itemName);
                //     break;
                //
                // case ItemSO.Type.armor: // LATER DELETE AND REPLACE WITH EQUIPMENT SYSTEM NOW ITS DEBUGGING ONLY
                //     Debug.Log("This is an armor: " + itemSO.itemName);
                //     break;
            }
        }

        private void ConsumeItem() {
            ItemConsumable item = itemSO as ItemConsumable;
            foreach (ItemConsumable.ModifyStat modifyStat in item.statsToModify) { Debug.Log(modifyStat.stat + " changed by " + modifyStat.value); }

            UpdateAmount(-1, true);
        }

        public void DropItems() { // CHANGE THIS SO IT WOULD DROP OUT OF CHARACTER BASED ON ITS FACING DIRECTION
            Item item = Instantiate(itemSO.itemPrefab, PlayerController.instance.transform.position + Vector3.right, Quaternion.identity).GetComponent<Item>();
            item.amount = amount;
            Destroy(gameObject);
        }

        public int UpdateAmount(int i, bool resetHeldItem) {
            int amountBefore = amount;
            amount += i;
            if (amount > itemSO.stackLimit) { amount = itemSO.stackLimit; }

            if (amount <= 0) {
                if (resetHeldItem == true) {
                    if (slot != null) slot.containedItem = null;
                    UIInput.instance.ResetHeldItem();
                }

                Destroy(gameObject);
            }

            isFull = amount >= itemSO.stackLimit;
            amountText.text = amount.ToString();

            return amount - amountBefore;
        }
    }
}