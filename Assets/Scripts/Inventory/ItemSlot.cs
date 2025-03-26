using UnityEngine;

public class ItemSlot : MonoBehaviour {
    public UIItem containedItem;

    public int AddItem(int amount) {
        return containedItem.AddItem(amount);
    }
    
    public int AddItem(GameObject uiItemPrefab, int amount) {
        containedItem = Instantiate(uiItemPrefab, gameObject.transform).GetComponent<UIItem>();
        containedItem.slot = this;
        return containedItem.AddItem(amount);
    }
}
