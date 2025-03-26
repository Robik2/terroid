using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIInput : MonoBehaviour {
    public static UIInput instance;
    private void Awake() {
        if (instance == null) {
            instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    public GraphicRaycaster raycaster;
    public EventSystem eventSystem;

    private bool isHoldingItem;
    private UIItem heldItem;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            LeftMouseClick();
        }
    }

    private void LeftMouseClick() {
        PointerEventData pointerEventData = new PointerEventData(eventSystem)
        {
            position = Input.mousePosition
        };
        
        List<RaycastResult> results = new List<RaycastResult>();
        raycaster.Raycast(pointerEventData, results);
        //END OF GETTING INPUT
        
        
        if(isHoldingItem && results.Count == 0) {heldItem.UseItem();} // USE ITEM
        
        foreach (var result in results)
        {
            if (result.gameObject.CompareTag("Slot")) {
                ItemSlot slot = result.gameObject.GetComponent<ItemSlot>();
                UIItem item = slot.containedItem;

                if (heldItem != null && slot == heldItem.slot) { // RETURN TO ITS SLOT
                    ToggleHold(heldItem); 
                    return;
                }
                
                if (InventoryManager.instance.menuActive == false) { // SELECT SLOT IN HOTBAR
                    print("YOU WANTED TO SELECT SLOT IN HOTBAR BUT ITS NOT CODED IN");
                    return;
                }
                
                switch (slot.containedItem, isHoldingItem) {
                    case (not null, false): // PICK UP ITEM
                        ToggleHold(item);
                        break;
                    
                    case (null, true): // PLACE ITEM ON EMPTY SLOT
                        PlaceItem(slot, heldItem);
                        break;
                    
                    case (not null, true): // SWAPPING ITEMS
                        SwapItem(slot, heldItem, item);
                        break;
                }
            }
        }
    }

    private void ToggleHold(UIItem item) { // WHEN NOT SWITCHING
        item.ToggleHold();
        isHoldingItem = !isHoldingItem;
        heldItem = isHoldingItem ? item : null;
        
        item.transform.SetParent(isHoldingItem ? item.transform.root : item.slot.transform);
    }
    
    private void ToggleHold(UIItem item, ItemSlot newSlot) { // WHEN SWITCHING TO OTHER EMPTY SLOT
        item.ToggleHold();
        isHoldingItem = !isHoldingItem;
        
        item.transform.SetParent(isHoldingItem ? item.transform.root : newSlot.transform);
    }

    private void PlaceItem(ItemSlot slot, UIItem item) {
        ToggleHold(item, slot);
        
        if(item.slot != null) {item.slot.containedItem = null;}
        slot.containedItem = item;
        item.slot = slot;
    }

    private void SwapItem(ItemSlot slot, UIItem item, UIItem slotItem) {
        if (item.slot != null) { item.slot.containedItem = null; }
        slot.containedItem = item;

        item.slot = slot;
        slotItem.slot = null;
        
        item.ToggleHold();
        slotItem.ToggleHold();
        heldItem = slotItem;
        
        item.transform.SetParent(slot.transform);
        slotItem.transform.SetParent(slotItem.transform.root);
    }

    public void PutItemBackToSlot() { // CALLED WHEN CLOSING INV WHILE HOLDING ITEM
        if (isHoldingItem == false) { return; }
        
        ToggleHold(heldItem);
    }
}
