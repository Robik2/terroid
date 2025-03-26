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

    private bool RMB;
    public float RMBTime;
    private float RMBLastCall;
    private float RMBCurrentRefreshTime;
    private readonly Dictionary<string, float> RMBRefreshTime = new Dictionary<string, float> {
        {"slow", .25f},
        {"medium", .16f},
        {"fast", .03f}
    };
    
    private void Update() {
        if (Input.GetMouseButtonDown(0)) { LeftMouseClick(); }

        RMBInput();
    }

    private void RMBInput() {
        if (Input.GetMouseButtonUp(1)) { RMB = false; }
        if (Input.GetMouseButtonDown(1)) { 
            RMB = true;
            RMBTime = Time.time;
            RMBLastCall = 1;
        }

        if (RMB == true) {
            switch (Time.time - RMBTime) {
                case < 1:
                    RMBCurrentRefreshTime = RMBRefreshTime["slow"];
                    break;
                
                case < 2.5f:
                    RMBCurrentRefreshTime = RMBRefreshTime["medium"];
                    break;
                
                default:
                    RMBCurrentRefreshTime = RMBRefreshTime["fast"];
                    break;
            }
            
            if(Time.time - RMBLastCall >= RMBCurrentRefreshTime) RightMouseClick();
        }
    }


    private void RightMouseClick() {
        RMBLastCall = Time.time;
        
        List<RaycastResult> results = GetObject(); // THIS SEARCHES FOR OBJECTS UNDER MOUSE

        if (isHoldingItem == true && results.Count == 0) { DropItems(); } // DROPS ITEMS ON THE GROUND

        foreach (var result in results) {
            if (result.gameObject.CompareTag("UIItem")) {
                UIItem item = result.gameObject.GetComponent<UIItem>();
                
                if (isHoldingItem == false) {
                    TakeOneItem(item);
                }else if (isHoldingItem == true && item.itemSO.itemName == heldItem.itemSO.itemName) {
                    AddOneItem(item);
                }
            }
        }
    }

    private void LeftMouseClick() {
        List<RaycastResult> results = GetObject(); // THIS SEARCHES FOR OBJECTS UNDER MOUSE
        
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

    private List<RaycastResult> GetObject() {
        PointerEventData pointerEventData = new PointerEventData(eventSystem) { position = Input.mousePosition };
        
        List<RaycastResult> results = new List<RaycastResult>();
        raycaster.Raycast(pointerEventData, results);
        
        return results;
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
        ItemSlot oldSlot = null;
        if (item.slot != null) {
            oldSlot = item.slot;
            if (item.slot.containedItem.isDividedByRMB == false) { item.slot.containedItem = null; }
        }
        
        ToggleHold(item, slot);
        
        slot.containedItem = item;

        item.slot = slot;
        
        if(oldSlot != null && oldSlot.containedItem != null) oldSlot.containedItem.isDividedByRMB = false;
    }

    private void SwapItem(ItemSlot slot, UIItem item, UIItem slotItem) {
        if (item.slot != null) { item.slot.containedItem = null; }
        slot.containedItem = item;

        item.slot = slot;
        slotItem.slot = null;
        
        item.ToggleHold();
        if(slotItem != null) slotItem.ToggleHold();
        heldItem = slotItem;
        
        item.transform.SetParent(slot.transform);
        slotItem.transform.SetParent(slotItem.transform.root);
    }

    public void PutItemBackToSlot() { // CALLED WHEN CLOSING INV WHILE HOLDING ITEM
        if (isHoldingItem == false) { return; }
        
        ToggleHold(heldItem);
    }

    public void ResetHeldItem() {
        isHoldingItem = false;
        heldItem = null;
    }

    private void DropItems() {
        heldItem.DropItems();
        ResetHeldItem();
    }

    private void TakeOneItem(UIItem item) {
        UIItem newItem = InventoryManager.instance.CreateUiItem(item.itemSO, item.transform);
        newItem.slot = item.slot;
        ToggleHold(newItem);
        
        heldItem.UpdateAmount(1, false);
        item.UpdateAmount(-1, false); // NAJPEWNIEJ TUTAJ JEST PROBLEM BO USUWA JAK JEST POJEDYNCZY ITEM A POTEM W STWAP ITEM CHCE GO DOSIEGNAC
        item.isDividedByRMB = true;
        print("1");
    }

    private void AddOneItem(UIItem item) {
        heldItem.UpdateAmount(1, false);
        item.UpdateAmount(-1, false);
        print("2");
    }
}
