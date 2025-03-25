using UnityEngine;

public class InventoryManager : MonoBehaviour {
    public static InventoryManager instance;
    private void Awake() {
        if (instance == null) {
            instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    [SerializeField] private GameObject inventoryMenu;
    [HideInInspector] public bool menuActive;
    public ItemSlot[] itemSlot;
    public ItemSO[] itemSOs;
    public GameObject emptyItemPrefab;
    
    private void Update()
    {
        if (Input.GetButtonDown("Cancel")) {
            inventoryMenu.SetActive(!menuActive);
            menuActive = !menuActive;
            if(menuActive == false) DeselectAllInventorySlots();
        }
    }

    public bool UseItem(string itemName) {
        foreach(ItemSO so in itemSOs)
        {
            if (so.itemName == itemName) {
                return so.UseItem();
            }
        }

        return false;
    }
    
    public int AddItem(GameObject uiItemPrefab, int amount) {
        var slot = SearchForSlot(uiItemPrefab);

        if (slot.slot == null) { return amount;}   
        
        int leftOverItems = slot.stackFound ? slot.slot.AddItem(amount) : slot.slot.AddItem(uiItemPrefab, amount);
        if (leftOverItems > 0) {
            leftOverItems = AddItem(uiItemPrefab, leftOverItems);
        }
        return leftOverItems;
        
        // foreach (ItemSlot slot in itemSlot) {
        //     if (slot.containedItem != null && slot.containedItem.itemName == uiItemPrefab.GetComponent<UIItem>().itemName && slot.containedItem.isFull == false) {
        //         
        //     }
        // }
        //
        // foreach(ItemSlot slot in itemSlot)
        // {
        //     if (slot.containedItem == null) {
        //         int leftOverItems = slot.AddItem(uiItemPrefab, amount);
        //         if (leftOverItems > 0) {
        //             leftOverItems = AddItem(uiItemPrefab, leftOverItems);
        //         }
        //         return leftOverItems;
        //     } else if (slot.containedItem.isFull == false && slot.containedItem.itemName == uiItemPrefab.GetComponent<UIItem>().itemName) {
        //         
        //     }
        // }
    }

    private (ItemSlot slot, bool stackFound) SearchForSlot(GameObject uiItemPrefab) {
        ItemSlot foundSlot = null;
        foreach (ItemSlot slot in itemSlot) {
            if (slot.containedItem != null && slot.containedItem.itemName == uiItemPrefab.GetComponent<UIItem>().itemName && slot.containedItem.isFull == false) {
                return (slot, true);
            }
            
            if (slot.containedItem == null && foundSlot == null) {
                foundSlot = slot;
            }
        }

        return (foundSlot, false);
    }

    public void DeselectAllInventorySlots() {
        for (int i = 10; i < itemSlot.Length; i++) {
            // DISABLE ALL ITEM HOVERS
        }
    }
}
