using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIInput : MonoBehaviour
{
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
        
        if(isHoldingItem && results.Count == 0) {print("USE ITEM HERE");}
        
        foreach (var result in results)
        {
            if (result.gameObject.CompareTag("Slot")) {
                ItemSlot slot = result.gameObject.GetComponent<ItemSlot>();
                UIItem item = slot.containedItem;

                if (heldItem != null && slot == heldItem.slot) { ToggleHold(heldItem); // RETURN TO ITS SLOT
                    return;
                }
                
                switch (slot.containedItem, isHoldingItem) {
                    case (not null, false): // PICK UP ITEM
                        ToggleHold(item);
                        break;
                    
                    case (null, true): // PLACE ITEM ON EMPTY SLOT
                        PlaceItem(slot, heldItem);
                        break;
                }

                return;
            }
        }
    }

    private void ToggleHold(UIItem item) { // WHEN NOT SWITCHING
        item.ToggleHold();
        isHoldingItem = !isHoldingItem;
        heldItem = isHoldingItem ? item : null;
        
        item.transform.SetParent(isHoldingItem ? item.transform.root : item.slot.transform);
        print("first TOGGLE");
    }
    
    private void ToggleHold(UIItem item, ItemSlot newSlot) { // WHEN SWITCHING TO OTHER EMPTY SLOT
        item.ToggleHold();
        isHoldingItem = !isHoldingItem;
        
        item.transform.SetParent(isHoldingItem ? item.transform.root : newSlot.transform);
        print("second TOGGLE");
    }

    private void PlaceItem(ItemSlot slot, UIItem item) {
        ToggleHold(item, slot);
        
        item.slot.containedItem = null;
        slot.containedItem = item;
        item.slot = slot;
    }
}
