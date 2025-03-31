using Sirenix.OdinInspector;
using UnityEngine;

namespace HealthAndStats {
    public class HealthManager : MonoBehaviour {
        [SerializeField] private int maxHealth;
        [SerializeField] [LabelText("IFrames (s)")] private float iframes;
        private float lastTimeHit;
        
        [SerializeField] [ReadOnly] [LabelText("MaxHP+Bonus")]
        [BoxGroup("Debug Health Stats")] [HorizontalGroup("Debug Health Stats/Row")]
        private int maxHealthAfterBonus;
        
        [SerializeField] [ReadOnly] [LabelText("CurrentHP")]
        [BoxGroup("Debug Health Stats")] [HorizontalGroup("Debug Health Stats/Row")]
        private int currentHealth;

        private StatsManager stats;

        private void Start() {
            stats = GetComponent<StatsManager>();
            InitialiseHealth();
        }

        public void InitialiseHealth() {
            maxHealthAfterBonus = maxHealth + stats.MaxHealthBonus;
            currentHealth = maxHealthAfterBonus;
        }

        public void TryTakeDamage(int amount) {
            if (Time.time - lastTimeHit > iframes) {
                lastTimeHit = Time.time;
                TakeDamage(amount);
            }
        }
        
        private void TakeDamage(int amount) {
            amount = amount - stats.Defense < 1 ? 1 : amount - stats.Defense; // APPLIES DEFENSE BUT CANT GO LOWER THAN 1
            currentHealth -= amount;
            Debug.Log("Took " + amount + " damage\nHealth left: " + currentHealth);
            DeathCheck();
        }

        private void DeathCheck() {
            if (currentHealth > 0) return;
            
            gameObject.SetActive(false); // ADD DEATH ANIMATION INSTEAD OF DISABLING OBJECT
        }
    }
}