using System.Collections;
using ObjectPooling;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Inventory {
    public class Item : MonoBehaviour {
        [ShowIf("@IsStackable")]
        public int amount = 1;
        [SerializeField] private float dropForce;
        [SerializeField] private ItemSO itemSO;
        [SerializeField] private Collider2D collectCol;
        [SerializeField] private ParticleSystem itemHighlight;

        private float spawnTime;

        private void OnEnable() {
            spawnTime = Time.time;
            amount = itemSO.isStackable ? amount : 1;
            SetHighlightColor(InventoryManager.rarityColors[itemSO.rarity.ToString()]);
            StartCoroutine(AllowCollecting());
        }

        private void OnDisable() {
            collectCol.excludeLayers = LayerMask.GetMask("Player");
        }

        private void SetHighlightColor(Color color) {
            var main = itemHighlight.main;
            main.startColor = color;
        }

        private IEnumerator AllowCollecting() {
            yield return new WaitForSeconds(0.5f);
            collectCol.excludeLayers = 0;
        }

        private void OnTriggerEnter2D(Collider2D other) {
            if (other.CompareTag("Player")) {
                int leftOverItems = InventoryManager.instance.AddItem(itemSO, amount);
                if (leftOverItems <= 0) {
                    // Destroy(gameObject);
                    ObjectPoolingManager.ReturnObjectToPool(gameObject);
                } else {
                    amount = leftOverItems;
                }
            } else if (other.CompareTag("Item")) {
                if (itemSO.isStackable == false) { return; }
                
                Item item = other.GetComponent<Item>();
                
                if (gameObject.GetComponent<Item>().spawnTime < item.spawnTime && item.itemSO.itemName == itemSO.itemName) {
                    item.amount += amount;
                    // Destroy(gameObject);
                    ObjectPoolingManager.ReturnObjectToPool(gameObject);
                }
            }
        }

        public void DropItem(Vector2 direction, int newAmount) { // CALLED WHEN THROWING ITEM FROM INVENTORY
            amount = newAmount;
            GetComponent<Rigidbody2D>().AddForce(direction * dropForce, ForceMode2D.Impulse);
        }

        private bool IsStackable => itemSO == null || itemSO.isStackable;
    }
}