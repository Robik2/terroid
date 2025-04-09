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
        public int MaxHealth => Mathf.RoundToInt(CalculateBonuses(ManagerHolder.instance.healthManager.MaxHealth, ItemSO.StatToChange.maxHealth));

        
        public int MaxMana => Mathf.RoundToInt(CalculateBonuses(ManagerHolder.instance.manaManager.MaxMana, ItemSO.StatToChange.maxMana));

        
        [SerializeField] private int defense;
        public int Defense => Mathf.RoundToInt(CalculateBonuses(defense, ItemSO.StatToChange.defense));

        
        [SerializeField] [LabelText("Crit Chance Bonus (%)")]
        private int critChanceBonus;
        public int CritChanceBonus => Mathf.RoundToInt(CalculateBonuses(critChanceBonus, ItemSO.StatToChange.critChanceBonus));

        
        [SerializeField] [LabelText("Melee Damage Multiplier")]
        private float meleeDamageMult = 1;
        public float MeleeDamageMult => CalculateBonuses(meleeDamageMult, ItemSO.StatToChange.meleeDamageMult);

        
        [SerializeField] [LabelText("Range Damage Multiplier")]
        private float rangeDamageMult = 1;
        public float RangeDamageMult => CalculateBonuses(rangeDamageMult, ItemSO.StatToChange.rangeDamageMult);

        
        [SerializeField] [LabelText("Magic Damage Multiplier")]
        private float magicDamageMult = 1;
        public float MagicDamageMult => CalculateBonuses(magicDamageMult, ItemSO.StatToChange.magicDamageMult);

        
        [SerializeField] [LabelText("Attack Speed Multiplier")]
        private float attackSpeedMult = 1;
        public float AttackSpeedMult => CalculateBonuses(attackSpeedMult, ItemSO.StatToChange.attackSpeedMult);

        
        [SerializeField] private float moveSpeedBonus;
        public float MoveSpeedBonus => CalculateBonuses(moveSpeedBonus, ItemSO.StatToChange.moveSpeedBonus);
    #endregion
        
        private Dictionary<string, Buff> allActiveBuffs = new();
        
        public static readonly HashSet<ItemSO.StatToChange> percentageStatChange = new() { ItemSO.StatToChange.meleeDamageMult, ItemSO.StatToChange.rangeDamageMult, ItemSO.StatToChange.magicDamageMult };
        
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
                UpdateHealthAndMana(statToChange);
            }
        }

        private void UpdateHealthAndMana(ItemSO.StatToChange statToChange) {
            switch (statToChange) { // UPDATES MAX HEALTH AND MANA
                case ItemSO.StatToChange.maxHealth:
                    ManagerHolder.instance.healthManager.UpdateHealth();
                    break;
                case ItemSO.StatToChange.maxMana:
                    ManagerHolder.instance.manaManager.UpdateMana();
                    break;
            }
        }

        private float CalculateBonuses(float statValue, ItemSO.StatToChange stat) {
            float bonus = 0;
            List<float> mults = new();
            
            foreach (KeyValuePair<string, Buff> buff in allActiveBuffs) {
                if (buff.Value.stat != stat) continue;
                
                if (buff.Value.isMult) mults.Add(buff.Value.value/100f);
                else bonus += buff.Value.value;
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
            
#if UNITY_EDITOR
            if(Input.GetKeyDown(KeyCode.L)) // STATS DEBUGGING
                Debug.Log($"Max Health: {ManagerHolder.instance.healthManager.MaxHealthAfterBonus}\n" +
                          $"Max Mana: {ManagerHolder.instance.manaManager.MaxManaAfterBonus}\n" +
                          $"Defense: {ManagerHolder.instance.statsManager.Defense}\n" +
                          $"Crit Chance Bonus: {ManagerHolder.instance.statsManager.CritChanceBonus}\n" +
                          $"Melee Damage Mult: {ManagerHolder.instance.statsManager.MeleeDamageMult}\n" +
                          $"Range Damage Mult: {ManagerHolder.instance.statsManager.RangeDamageMult}\n" +
                          $"Magic Damage Mult: {ManagerHolder.instance.statsManager.MagicDamageMult}\n" +
                          $"Attack Speed Mult: {ManagerHolder.instance.statsManager.AttackSpeedMult}\n" +
                          $"Move Speed Bonus: {ManagerHolder.instance.statsManager.MoveSpeedBonus}");
#endif
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