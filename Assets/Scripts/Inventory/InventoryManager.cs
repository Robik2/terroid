using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

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
    [HideInInspector] public bool isHoveringOverSlot;
    public ItemSlot[] hotbarSlots;
    public ItemSlot[] inventorySlots;
    public GameObject ItemUIPrefab;
    [ReadOnly] public ItemSlot selectedSlot;

    private void Start() {
        SelectSlot(hotbarSlots[0]);
    }

    private void Update()
    {
        if (Input.GetButtonDown("Cancel")) {
            inventoryMenu.SetActive(!menuActive);
            menuActive = !menuActive;
            UIInput.instance.RMB = false;
            if(menuActive == false) UIInput.instance.PutItemBackToSlot();
        }
    }
    
    public int AddItem(ItemSO itemSO, int amount) {
        var results = SearchForSlot(itemSO);

        if (results.slot == null) { return amount;}   
        
        int leftOverItems = results.stackFound ? results.slot.AddItem(amount) : results.slot.AddItem(itemSO, amount, CreateUiItem(itemSO, results.slot.transform));
        if (leftOverItems > 0) {
            leftOverItems = AddItem(itemSO, leftOverItems);
        }
        return leftOverItems;
    }

    private (ItemSlot slot, bool stackFound) SearchForSlot(ItemSO itemSO) {
        ItemSlot foundSlot = null;
        foreach (ItemSlot slot in hotbarSlots) {
            if (slot.containedItem != null && slot.containedItem.itemSO.itemName == itemSO.itemName && slot.containedItem.isFull == false) {
                return (slot, true);
            }
            
            if (slot.containedItem == null && foundSlot == null) {
                foundSlot = slot;
            }
        }

        foreach (ItemSlot slot in inventorySlots) {
            if (slot.containedItem != null && slot.containedItem.itemSO.itemName == itemSO.itemName && slot.containedItem.isFull == false) {
                return (slot, true);
            }
            
            if (slot.containedItem == null && foundSlot == null) {
                foundSlot = slot;
            }
        }
        
        return (foundSlot, false);
    }

    public UIItem CreateUiItem(ItemSO itemSO, Transform parent) {
        UIItem item = Instantiate(ItemUIPrefab, parent).GetComponent<UIItem>();
        item.GetComponent<Image>().sprite = itemSO.sprite;
        item.amountText.enabled = !itemSO.isStackable;
        item.name = itemSO.itemName;
        item.itemSO = itemSO;

        return item;
    }

    public void SelectSlot(ItemSlot slot) {
        foreach (ItemSlot hotbarSlot in hotbarSlots) { hotbarSlot.DeselectSlot(); }
        
        selectedSlot = slot;
        selectedSlot.SelectSlot();
    }
}
