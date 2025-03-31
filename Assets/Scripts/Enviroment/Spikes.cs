using HealthAndStats;
using UnityEngine;

namespace Environment {
    public class Spikes : MonoBehaviour {
        [SerializeField] private int damage;

        private void OnTriggerStay2D(Collider2D other) {
            if (other.CompareTag("Player") && other.TryGetComponent(out HealthManager healthManager)) {
                healthManager.TryTakeDamage(damage);
            }
        }
    }
}