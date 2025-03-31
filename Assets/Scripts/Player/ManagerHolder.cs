using UnityEngine;

namespace HealthAndStats {
    public class ManagerHolder : MonoBehaviour {
        public static ManagerHolder instance;
        private void Awake() {
            if (instance == null) { instance = this; } else { Destroy(gameObject); }
        }

        public HealthManager healthManager;
        public ManaManager manaManager;
        public StatsManager statsManager;
    }
}