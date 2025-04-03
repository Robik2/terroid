using Sirenix.OdinInspector;
using UnityEngine;

namespace HealthAndStats {
    public class ManaManager : MonoBehaviour {
        [SerializeField] private float maxMana;
        public float MaxMana => maxMana;
        
        [SerializeField] [ReadOnly] [LabelText("MaxMP+Bonus")]
        [BoxGroup("Debug Health Stats")] [HorizontalGroup("Debug Health Stats/Row")]
        private float maxManaAfterBonus;
        public float MaxManaAfterBonus => maxManaAfterBonus;
        
        [SerializeField] [ReadOnly] [LabelText("CurrentMP")]
        [BoxGroup("Debug Health Stats")] [HorizontalGroup("Debug Health Stats/Row")]
        private float currentMana;

        private StatsManager stats;

        private void Start() {
            stats = GetComponent<StatsManager>();
            InitialiseMana();
        }

        public void InitialiseMana() {
            maxManaAfterBonus = stats.MaxMana;
            currentMana = maxManaAfterBonus;
        }
        
        public void UpdateMana() {
            maxManaAfterBonus = stats.MaxMana;
        }

        public bool RestoreMana(int value) {
            if (currentMana >= maxManaAfterBonus) return false;
            
            currentMana += value;
            currentMana = currentMana > maxManaAfterBonus ? maxManaAfterBonus : currentMana;
            return true;
        }
        
        private bool ManaCheck(int manaReq) {
            if (currentMana < manaReq) return false;

            currentMana -= manaReq;
            return true;
        }
        
    #if UNITY_EDITOR
        private void Update() {
            if (Input.GetMouseButtonDown(1)) { // USING MANA UNTIL ACTUAL ITEMS THAT USE MANA      DELETE ME LATER
                ManaCheck(5);
            }
        }
#endif
    }
}