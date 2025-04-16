using Inventory;
using Sirenix.OdinInspector;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace HealthAndStats {
    public class StatsManager : MonoBehaviour {
    #region Stats Variables
        [InfoBox("After finishing debugging on player reset component to base values")] 
        public int MaxHealth => Mathf.RoundToInt(CalculateBonuses(GetComponent<HealthManager>().MaxHealth, ItemSO.StatToChange.MaxHealth));

        
        public int MaxMana => Mathf.RoundToInt(CalculateBonuses(GetComponent<ManaManager>().MaxMana, ItemSO.StatToChange.MaxMana));

        
        [SerializeField] private int defense;
        public int Defense => Mathf.RoundToInt(CalculateBonuses(defense, ItemSO.StatToChange.Defense));

        
        [SerializeField] [LabelText("Crit Chance Bonus (%)")]
        private int critChanceBonus;
        public int CritChanceBonus => Mathf.RoundToInt(CalculateBonuses(critChanceBonus, ItemSO.StatToChange.CritChanceBonus));

        
        [SerializeField] [LabelText("Melee Damage Multiplier")]
        private float meleeDamageMult = 1;
        public float MeleeDamageMult => CalculateBonuses(meleeDamageMult, ItemSO.StatToChange.MeleeDamage);

        
        [SerializeField] [LabelText("Range Damage Multiplier")]
        private float rangeDamageMult = 1;
        public float RangeDamageMult => CalculateBonuses(rangeDamageMult, ItemSO.StatToChange.RangeDamage);

        
        [SerializeField] [LabelText("Magic Damage Multiplier")]
        private float magicDamageMult = 1;
        public float MagicDamageMult => CalculateBonuses(magicDamageMult, ItemSO.StatToChange.MagicDamage);

        
        [SerializeField] [LabelText("Attack Speed Multiplier")]
        private float attackSpeedMult = 1;
        public float AttackSpeedMult => CalculateBonuses(attackSpeedMult, ItemSO.StatToChange.AttackSpeed);

        
        [SerializeField] private float moveSpeedBonus;
        public float MoveSpeedBonus => CalculateBonuses(moveSpeedBonus, ItemSO.StatToChange.MoveSpeedBonus);
    #endregion
        
        private Dictionary<string, Buff> allActiveBuffs = new();
        private Dictionary<string, Dictionary<ItemSO.StatToChange, float>> equipedItems = new();
        
        public static readonly HashSet<ItemSO.StatToChange> percentageStatChange = new() { ItemSO.StatToChange.MeleeDamage, ItemSO.StatToChange.RangeDamage, ItemSO.StatToChange.MagicDamage, ItemSO.StatToChange.AttackSpeed };

        public void ApplyItemStats(List<ItemArmor.ModifyStat> stats, string itemName) {
            Dictionary<ItemSO.StatToChange, float> itemStats = new();
            foreach (ItemArmor.ModifyStat stat in stats) {
                print(stat.stat);
                itemStats.Add(stat.stat, stat.value);
            }
            
            equipedItems.Add(itemName, itemStats);
            UpdateHealthAndMana();
        }

        public void DiscardItemStats(string itemName) {
            equipedItems.Remove(itemName);
            UpdateHealthAndMana();
        }
        
        public void ApplyBuff(ItemSO.StatToChange statToChange, float newValue, float duration, bool newIsMult, string newBuffName) {
            // IF THE SAME BUFF IS ACTIVE THEN REFRESH IT
            if (allActiveBuffs.TryGetValue(newBuffName, out Buff buff)) {
                buff.buffStartTime = Time.time;
            } else {
                buff = new Buff() {
                    buffName = newBuffName,
                    buffStartTime = Time.time,
                    buffDuration = duration,
                    isMult = newIsMult,
                    stat = statToChange,
                    value = newValue
                };
                
                allActiveBuffs.Add(newBuffName, buff);
                UpdateHealthAndMana();
            }
        }

        private void UpdateHealthAndMana() {
            ManagerHolder.instance.healthManager.UpdateHealth();
            ManagerHolder.instance.manaManager.UpdateMana();
        }

        private float CalculateBonuses(float statValue, ItemSO.StatToChange stat) {
            float bonus = 0;
            List<float> mults = new();

            foreach (Dictionary<ItemSO.StatToChange, float> itemStat in equipedItems.Values) {
                foreach (KeyValuePair<ItemSO.StatToChange, float> stats in itemStat) {
                    if (stats.Key != stat) continue;

                    bonus += stats.Value;
                }
            }
            
            foreach (KeyValuePair<string, Buff> buff in allActiveBuffs) {
                if (buff.Value.stat != stat) continue;
                
                if (buff.Value.isMult) mults.Add(buff.Value.value/100f);
                else bonus += buff.Value.value/100f;
            }

            statValue += bonus;
            foreach (float m in mults) {
                statValue *= 1+m;
            }
            
            return statValue;
        }

        private void BuffTimer() {
            List<string> expiredBuffs = new();

            foreach (KeyValuePair<string, Buff> buff in allActiveBuffs) {
                if (Time.time - buff.Value.buffStartTime >= buff.Value.buffDuration) expiredBuffs.Add(buff.Key);
            }

            foreach (string s in expiredBuffs) {
                allActiveBuffs.Remove(s);
            }
        }
        
        private void Update() {
            BuffTimer();
            // DisplayBuffOnUI();
            //DO THIS ONE LATER
        }
    }
    
    
    public class Buff {
        public string buffName;
        public Sprite buffIcon;
        public float buffStartTime;
        public float buffDuration;
        public ItemSO.StatToChange stat;
        public float value;
        public bool isMult;
    }
}