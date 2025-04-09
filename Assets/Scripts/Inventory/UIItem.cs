using ObjectPooling;
using Player;
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

        public void DropItems() {
            // Item item = Instantiate(itemSO.itemPrefab, PlayerController.instance.transform.position, Quaternion.identity).GetComponent<Item>();
            Item item = ObjectPoolingManager.SpawnObject(itemSO.itemPrefab.gameObject, PlayerController.instance.transform.position, Quaternion.identity, ObjectPoolingManager.PoolingParent.Item).GetComponent<Item>();
            item.DropItem(PlayerController.instance.GetFacingDirection(), amount);
            ObjectPoolingManager.ReturnObjectToPool(gameObject, true);
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

                // Destroy(gameObject);
                ObjectPoolingManager.ReturnObjectToPool(gameObject,true);
            }

            isFull = amount >= itemSO.stackLimit;
            amountText.text = amount.ToString();

            return amount - amountBefore;
        }
    }
}