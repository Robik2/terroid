using UnityEngine;

namespace ObjectPooling {
    public class ReturnProjectile : MonoBehaviour {
        public void ReturnToPool() {
            ObjectPoolingManager.ReturnObjectToPool(gameObject, true);
        }
    }
}