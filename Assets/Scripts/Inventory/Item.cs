using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

public class Item : MonoBehaviour {
    [ShowIf("@IsStackable")]
    public int amount = 1;
    [SerializeField] private ItemSO itemSO;
    [SerializeField] private Collider2D collectCol;

    private float spawnTime;

    private void Start() {
        spawnTime = Time.time;
        amount = itemSO.isStackable ? amount : 1;
        StartCoroutine(AllowCollecting());
    }

    private IEnumerator AllowCollecting() {
        yield return new WaitForSeconds(0.5f);
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
            if (itemSO.isStackable == false) { return; }
            
            Item item = other.GetComponent<Item>();
            
            if (gameObject.GetComponent<Item>().spawnTime < item.spawnTime && item.itemSO.itemName == itemSO.itemName) {
                item.amount += amount;
                Destroy(gameObject);
            }
        }
    }

    private bool IsStackable => itemSO.isStackable;
}
