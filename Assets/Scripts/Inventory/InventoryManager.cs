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
    private bool menuActive;
    public ItemSlot[] itemSlot;
    public ItemSO[] itemSOs;
    public GameObject emptyItemPrefab;
    
    private void Update()
    {
        if (Input.GetButtonDown("Cancel")) {
            inventoryMenu.SetActive(!menuActive);
            menuActive = !menuActive;
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
    
    public int AddItem(string itemName, string itemDescription, int amount, Sprite sprite) {
        foreach(ItemSlot slot in itemSlot)
        {
            if (slot.isFull == false && slot.itemName == itemName || slot.amount == 0) {
                int leftOverItems = slot.AddItem(itemName, itemDescription, amount, sprite);
                if (leftOverItems > 0) {
                    leftOverItems = AddItem(itemName, itemDescription, leftOverItems, sprite);
                }
                return leftOverItems;
            }
        }

        return amount;
    }

    public void DeselectAllSlots() {
        foreach(ItemSlot slot in itemSlot) {
            slot.selectedPanel.SetActive(false);
            slot.isSelected = false;
        }
    }
}
