using System.Collections.Generic;
using HealthAndStats;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Inventory {
    [CreateAssetMenu(fileName = "NewConsumable", menuName = "Scriptable Objects/Item Consumable SO")]
    public class ItemConsumable : ItemSO {
        public bool isHealingItem;
        public List<ModifyStat> statsToModify = new();

        public enum Restores {
            nothing,
            health,
            mana,
            healthAndMana
        }
        
        [System.Serializable]
        public struct ModifyStat {
            public StatToChange stat;
            public float value;
            public bool isBuff;

            [ShowIf("isBuff")] public bool isMult;

            [ShowIf("isBuff")] [LabelText("Buff Duration (s)")]
            public float buffDuration;
        }

        public override void UseItem() {
            foreach (ModifyStat stat in statsToModify) {
                if (stat.isBuff == false) {
                    RestoreStat(stat);
                    continue;
                }

                ManagerHolder.instance.statsManager.ApplyBuff(stat.stat, stat.value, stat.buffDuration, stat.isMult, stat.stat.ToString() + itemName.ToString());
            }
        }

        private void RestoreStat(ModifyStat stat) {
            if (stat.stat == StatToChange.maxHealth) ManagerHolder.instance.healthManager.RestoreHealth(stat.value);
            else ManagerHolder.instance.manaManager.RestoreMana(Mathf.RoundToInt(stat.value));
        }
    }
}