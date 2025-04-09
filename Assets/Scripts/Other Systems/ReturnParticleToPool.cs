using UnityEngine;

namespace ObjectPooling {
    public class ReturnParticleToPool : MonoBehaviour {
        private void OnParticleSystemStopped() {
            ObjectPoolingManager.ReturnObjectToPool(gameObject);
        }
    }
}