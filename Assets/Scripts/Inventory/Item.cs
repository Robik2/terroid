using UnityEngine;

public class Item : MonoBehaviour {
    public string itemName;
    public string itemDescription;
    public int amount;
    public Sprite sprite;

    private float spawnTime;

    private void Start() {
        spawnTime = Time.time;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            int leftOverItems = InventoryManager.instance.AddItem(itemName, itemDescription, amount, sprite);
            if (leftOverItems <= 0) {
                Destroy(gameObject);
            } else {
                amount = leftOverItems;
            }
        } else if (other.CompareTag("Item")) {
            Item item = other.GetComponent<Item>();
            if (gameObject.GetComponent<Item>().spawnTime < item.spawnTime) {
                item.amount += amount;
                Destroy(gameObject);
            }
        }
    }
}
