using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Inventory {
    public class UIInput : MonoBehaviour {
        public static UIInput instance;

        private void Awake() {
            if (instance == null) { instance = this; } else { Destroy(gameObject); }
        }

        public GraphicRaycaster raycaster;
        public EventSystem eventSystem;
        
        private UIItem heldItem;
        public UIItem HeldItem => heldItem;

        private bool isHoldingItem, isHeldItemDivided;
        public bool IsHoldingItem => isHoldingItem;

        
        private ItemSlot oldSlot;

        // RIGHT MOUSE BUTTON VARIABLES
        [HideInInspector] public bool RMB;
        private float RMBTime;
        private float RMBLastCall;
        private float RMBCurrentRefreshTime;

        private readonly Dictionary<string, float> RMBRefreshTime = new() {
            { "slow", .2f },
            { "medium", .08f },
            { "fast", .015f }
        };

        private void Update() {
            if (RMB == true) { // HOLDING ACTION OF RMB
                SetRMBRefreshTime();
                if (Time.time - RMBLastCall >= RMBCurrentRefreshTime) RightMouseClick();
            }
        }
        
    #region RMB    
        public void RMBInput(InputAction.CallbackContext context) {
            if (context.canceled) { RMB = false; }

            if (context.performed) {
                RMB = true;
                RMBTime = Time.time;
                RMBLastCall = 1;
            }
        }

        private void SetRMBRefreshTime() {
            RMBCurrentRefreshTime = (Time.time - RMBTime) switch {
                < .8f => RMBRefreshTime["slow"],
                < 1.5f => RMBRefreshTime["medium"],
                _ => RMBRefreshTime["fast"]
            };
        }

        private void RightMouseClick() {
            RMBLastCall = Time.time;

            if (InventoryManager.instance.menuActive == false) return;

            List<RaycastResult> results = GetObject(); // THIS SEARCHES FOR OBJECTS UNDER MOUSE

            if (isHoldingItem == true && results.Count == 0) { DropItems(); } // DROPS ITEMS ON THE GROUND

            foreach (var result in results) {
                if (result.gameObject.CompareTag("UIItem")) {
                    UIItem item = result.gameObject.GetComponent<UIItem>();

                    if (isHoldingItem == false) { TakeOneItem(item); } else if (isHoldingItem == true && item.itemSO.itemName == heldItem.itemSO.itemName) { AddOneItem(item); }
                }
            }
        }
    #endregion
    
        public void LMBInput(InputAction.CallbackContext context) {
            if (context.action.WasPerformedThisFrame()) {
                print("performed");
                List<RaycastResult> results = GetObject(); // THIS SEARCHES FOR OBJECTS UNDER MOUSE

                foreach (var result in results) {
                    if (result.gameObject.CompareTag("Slot")) {
                        ItemSlot slot = result.gameObject.GetComponent<ItemSlot>();
                        UIItem item = slot.containedItem;

                        if (heldItem != null && slot == heldItem.slot) { // RETURN TO ITS SLOT
                            ToggleHold(heldItem);
                            return;
                        }

                        if (InventoryManager.instance.menuActive == false) { // SELECT SLOT IN HOTBAR
                            InventoryManager.instance.SelectSlot(slot);
                            return;
                        }

                        switch (slot.containedItem, isHoldingItem) {
                            case (not null, false): // PICK UP ITEM
                                ToggleHold(item);
                                break;

                            case (null, true): // PLACE ITEM ON EMPTY SLOT
                                PlaceItem(slot, heldItem);
                                break;

                            case (not null, true): // ADDING TO STACK OR SWAPING ITEM IF CANT
                                if (heldItem.itemSO.itemName == item.itemSO.itemName && item.isFull == false && item.itemSO.isStackable == true) AddItemToStack(item, heldItem);
                                else SwapItem(slot, heldItem, item);
                                break;
                        }
                    }
                }
            }
        }

        private List<RaycastResult> GetObject() {
            PointerEventData pointerEventData = new PointerEventData(eventSystem) { position = Input.mousePosition };

            List<RaycastResult> results = new List<RaycastResult>();
            raycaster.Raycast(pointerEventData, results);

            return results;
        }

        private void ToggleHold(UIItem item) { // WHEN NOT SWITCHING
            item.ToggleHold();
            if (item.slot != null) {
                item.slot.containedItem = null;
                item.slot = null;
            }

            isHoldingItem = !isHoldingItem;
            isHeldItemDivided = isHoldingItem != false && isHeldItemDivided;
            heldItem = isHoldingItem ? item : null;

            item.transform.SetParent(isHoldingItem ? item.transform.root : item.slot.transform);
        }

        private void ToggleHold(UIItem item, ItemSlot newSlot) { // WHEN SWITCHING TO OTHER EMPTY SLOT
            item.ToggleHold();
            isHoldingItem = !isHoldingItem;
            heldItem = null;

            item.transform.SetParent(isHoldingItem ? item.transform.root : newSlot.transform);
        }

    #region ManagingItemSlot

        private void PlaceItem(ItemSlot slot, UIItem item) {
            if (item.slot != null && item.slot.containedItem != null) {
                if (item.slot.containedItem.isDividedByRMB == false) { item.slot.containedItem = null; }
            }

            ToggleHold(item, slot);

            slot.containedItem = item;

            item.slot = slot;

            if (oldSlot != null && oldSlot.containedItem != null) oldSlot.containedItem.isDividedByRMB = false;
            oldSlot = null;
            InventoryManager.instance.isHoveringOverSlot = true;
        }

        private void SwapItem(ItemSlot slot, UIItem item, UIItem slotItem) {
            if (item.slot != null) { item.slot.containedItem = null; }

            slot.containedItem = item;

            item.slot = slot;
            slotItem.slot = null;

            item.ToggleHold();
            if (slotItem != null) slotItem.ToggleHold();
            heldItem = slotItem;

            item.transform.SetParent(slot.transform);
            slotItem.transform.SetParent(slotItem.transform.root);
        }

        public void PutItemBackToSlot() { // CALLED WHEN CLOSING INV
            if (isHoldingItem == false) { return; }

            InventoryManager.instance.AddItem(heldItem.itemSO, heldItem.amount);

            Destroy(heldItem.gameObject);
            ResetHeldItem();
            oldSlot = null;
        }

        private void AddItemToStack(UIItem stack, UIItem item) {

            item.UpdateAmount(-stack.UpdateAmount(item.amount, false), true);
        }

    #endregion

        public void ResetHeldItem() {
            isHoldingItem = false;
            heldItem = null;
        }

        private void DropItems() {
            heldItem.DropItems();
            ResetHeldItem();
        }

        #region TakingItemsFromStack

        private void TakeOneItem(UIItem item) {
            oldSlot = item.slot;
            isHeldItemDivided = true;

            UIItem newItem = InventoryManager.instance.CreateUiItem(item.itemSO, item.transform);

            if (item.amount == 1) { item.slot.containedItem = null; }

            ToggleHold(newItem);

            item.isDividedByRMB = true;
            heldItem.UpdateAmount(1, false);
            item.UpdateAmount(-1, false);
        }

        private void AddOneItem(UIItem item) {
            if (item.itemSO.isStackable == false) return;
            if (heldItem.amount == heldItem.itemSO.stackLimit) return;

            if (item.amount == 1) { item.slot.containedItem = null; }

            heldItem.UpdateAmount(1, false);
            item.UpdateAmount(-1, false);
        }

        #endregion

    #region ChecksForOtherScripts

        public IEnumerator HoveringOverSlotCheck() { // THIS IS USED TO DISPLAY DESCRIPTION WHEN YOU COLLECT ITEM AND IT LANDS ON THE SLOT THAT YOU ARE HOVERING OVER
            yield return new WaitForSeconds(.01f);

            DelayedHoverCheck();
        }

        private void DelayedHoverCheck() {
            List<RaycastResult> results = GetObject();

            if (results.Count == 0) return;

            foreach (var result in results) {
                if (result.gameObject.CompareTag("UIItem")) {
                    ItemDescription.instance.UpdateDescription(result.gameObject.GetComponent<UIItem>().itemSO);
                    InventoryManager.instance.isHoveringOverSlot = true;
                    
                }
            }
        }
    #endregion
    }
}