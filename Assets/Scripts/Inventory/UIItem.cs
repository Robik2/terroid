using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIItem : MonoBehaviour{
    public string itemName;
    [SerializeField] private GameObject item;
    [SerializeField] private TMP_Text amountText;
    [HideInInspector] public int amount;
    [HideInInspector] public bool isFull;
    [SerializeField] private int stackLimit;
    public ItemSlot slot;
    private bool isHeld;
    [SerializeField] private ItemUsage usage;

    private void Update() {
        if (isHeld == true) {
            transform.position = Input.mousePosition;
        }
    }
    
    public int AddItem(int amountNew) {
        amount += amountNew;
        if (amount >= stackLimit) {
            amountText.text = stackLimit.ToString();
            isFull = true;
            
            int extraItems = amount - stackLimit;
            amount = stackLimit;
            return extraItems;
        }
        
        amountText.text = amount.ToString();
        return 0;
    }

    public void ToggleHold() {
        isHeld = !isHeld;
        GetComponent<Image>().raycastTarget = !isHeld;
    }

    public void UseItem() {
        usage.UseItem();
    }
}
