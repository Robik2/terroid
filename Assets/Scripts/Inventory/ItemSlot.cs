using Player;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Inventory {
    public class ItemSlot : MonoBehaviour, IPointerExitHandler, IPointerEnterHandler {
        public UIItem containedItem;
        [SerializeField] private Color selectedColor;
        private Color deselectedColor;
        private Image image;
        private bool isHoveredOver;

        private void Start() {
            image = GetComponent<Image>();
            deselectedColor = image.color;
        }

        public int AddItem(int amount) {
            return containedItem.AddItem(amount);
        }

        public int AddItem(ItemSO itemSO, int amount, UIItem item) {
            containedItem = item;
            containedItem.slot = this;
            
            return containedItem.AddItem(amount);
        }

        public void OnPointerExit(PointerEventData eventData) {
            isHoveredOver = false;
            UIInput.instance.RMB = false; // THIS RESETS RMB REFRESH RATE IF LEFT THE SLOT
            InventoryManager.instance.isHoveringOverSlot = false;
            InventoryManager.instance.SetCanUseItem(true);
        }

        public void OnPointerEnter(PointerEventData eventData) {
            isHoveredOver = true;
            InventoryManager.instance.isHoveringOverSlot = containedItem != null;
            InventoryManager.instance.SetCanUseItem(false);
            
            if (containedItem == null) return;
            

            ItemDescription.instance.UpdateDescription(containedItem.itemSO);
        }

        public void SelectSlot() {
            image.color = selectedColor;
        }

        public void DeselectSlot() {
            image.color = deselectedColor;
        }

        public void CheckHover() { // ENABLED PLAYER TO USE ITEM WHEN INVENTORY IS CLOSED WHILE HOVERING OVER INVENTORY SLOT
            if (isHoveredOver == true) {
                InventoryManager.instance.SetCanUseItem(true);
                isHoveredOver = false;
            }
        }
    }
}