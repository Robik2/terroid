using UnityEngine;
using UnityEngine.EventSystems;

public class ItemSlot : MonoBehaviour, IPointerExitHandler {
    public UIItem containedItem;

    public int AddItem(int amount) {
        return containedItem.AddItem(amount);
    }
    
    public int AddItem(ItemSO itemSO, int amount, UIItem item) {
        containedItem = item;
        containedItem.slot = this;
        
        return containedItem.AddItem(amount);
    }
    
    public void OnPointerExit(PointerEventData eventData) { // THIS RESETS RMB REFRESH RATE IF LEFT THE SLOT
        UIInput.instance.RMBTime = Time.time;
    }
}
