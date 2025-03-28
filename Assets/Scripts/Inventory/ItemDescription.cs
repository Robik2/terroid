using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemDescription : MonoBehaviour {
    public static ItemDescription instance;
    private void Awake() {
        if (instance == null) {
            instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    [SerializeField] private TMP_Text itemName;
    [SerializeField] private TMP_Text description;
    [SerializeField] private Image image;
    [SerializeField] private GameObject panel;

    private void Update() {
        panel.SetActive(InventoryManager.instance.menuActive && InventoryManager.instance.isHoveringOverSlot);
    
        transform.position = Input.mousePosition;
    }

    public void UpdateDescription(ItemSO itemSO) {
        itemName.text = itemSO.itemName;
        image.sprite = itemSO.sprite;

        string descriptionText = "";

        switch (itemSO.itemType) {
            case ItemSO.Type.consumable:
                foreach (ItemSO.ModifyStat modifyStat in itemSO.statsToModify) {
                    descriptionText += "Restores " + modifyStat.stat + " by " + modifyStat.value + "\n";
                }
                break;
        }
        
        description.text = descriptionText;
    }
}
