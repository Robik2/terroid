using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour, IPointerClickHandler {
    //ITEM DATA
    public string itemName;
    [SerializeField] private string itemDescription;
    public int amount;
    [SerializeField] private Sprite sprite;
    [SerializeField] private Sprite emptySprite;
    public bool isFull;

    //SLOT DATA
    [SerializeField] private TMP_Text amountText;
    [SerializeField] private Image itemImage;
    [SerializeField] private int stackLimit;
    public GameObject selectedPanel;
    public bool isSelected;

    //DESCRIPTION SLOT
    [SerializeField] private Image itemDescriptionImage;
    [SerializeField] private TMP_Text itemDescriptionNameText;
    [SerializeField] private TMP_Text itemDescriptionText;
    
    public int AddItem(string itemNameNew, string itemDescriptionNew, int amountNew, Sprite spriteNew) {
        if (isFull) { return amount;}
        
        itemName = itemNameNew;
        sprite = spriteNew;
        itemImage.sprite = sprite;
        itemDescription = itemDescriptionNew;
        amount += amountNew;
        if (amount >= stackLimit) {
            amountText.text = stackLimit.ToString();
            amountText.enabled = true;
            isFull = true;
            
            int extraItems = amount - stackLimit;
            amount = stackLimit;
            return extraItems;
        }

        amountText.text = amount.ToString();
        amountText.enabled = true;

        return 0;

        // if (isSelected) {
        //     itemDescriptionText.text = itemDescription;
        //     itemDescriptionNameText.text = itemName;
        //     itemDescriptionImage.sprite = sprite == null ? emptySprite : sprite;
        // }
    }

    public void OnPointerClick(PointerEventData eventData) {
        switch (eventData.button) {
            case PointerEventData.InputButton.Left:
                OnLeftClick();
                break;
            case PointerEventData.InputButton.Right:
                OnRightClick();
                break;
        }
    }

    private void OnLeftClick() {
        if (isSelected && amount > 0 && InventoryManager.instance.UseItem(itemName)) {
            amount--;
            amountText.text = amount.ToString();
            if (amount <= 0) {
                EmptySlot();
            }
        } else {
            InventoryManager.instance.DeselectAllSlots();
            selectedPanel.SetActive(true);
            isSelected = true;
                    
            itemDescriptionText.text = itemDescription;
            itemDescriptionNameText.text = itemName;
            itemDescriptionImage.sprite = sprite == null ? emptySprite : sprite;
        }
        
        
    }

    private void OnRightClick() {
        if (amount <= 0) { return;}
        
        Item newItem = Instantiate(InventoryManager.instance.emptyItemPrefab, PlayerController.instance.transform.position + new Vector3(2, 0, 0), Quaternion.identity).GetComponent<Item>();

        newItem.gameObject.name = itemName;
        
        newItem.amount = 1;
        newItem.itemName = itemName;
        newItem.itemDescription = itemDescription;
        newItem.sprite = sprite;

        SpriteRenderer sr = newItem.GetComponentInChildren<SpriteRenderer>();
        sr.sprite = sprite;
        sr.sortingOrder = 5;
        
        amount--;
        amountText.text = amount.ToString();
        if (amount <= 0) {
            EmptySlot();
        }
    }

    private void EmptySlot() {
        amountText.enabled = false;
        itemImage.sprite = emptySprite;
    
        itemDescriptionNameText.text = "";
        itemDescriptionText.text = "";
        itemDescriptionImage.sprite = emptySprite;

        sprite = null;
        itemName = null;
        itemDescription = null;
    }
}
