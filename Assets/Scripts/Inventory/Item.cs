using System.Collections;
using UnityEngine;

public class Item : MonoBehaviour {
    public int amount;
    [SerializeField] private ItemSO itemSO;
    [SerializeField] private Collider2D collectCol;

    private float spawnTime;

    private void Start() {
        spawnTime = Time.time;
        StartCoroutine(AllowCollecting());
    }

    private IEnumerator AllowCollecting() {
        yield return new WaitForSeconds(0.8f);
        collectCol.excludeLayers = 0;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            int leftOverItems = InventoryManager.instance.AddItem(itemSO, amount);
            if (leftOverItems <= 0) {
                Destroy(gameObject);
            } else {
                amount = leftOverItems;
            }
        } else if (other.CompareTag("Item")) {
            Item item = other.GetComponent<Item>();
            
            if (gameObject.GetComponent<Item>().spawnTime < item.spawnTime && item.itemSO.itemName == itemSO.itemName) {
                item.amount += amount;
                Destroy(gameObject);
            }
        }
    }
}
