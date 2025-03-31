using Inventory;
using Sirenix.OdinInspector;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace HealthAndStats {
    public class StatsManager : MonoBehaviour {
        [InfoBox("After finishing debugging on player reset component to base values")] 
        [SerializeField]
        private List<float> currentHealthBonuses = new();
        private List<float> currentHealthMultipliers = new();
        public int MaxHealth => Mathf.RoundToInt(CalculateBonuses(ManagerHolder.instance.healthManager.MaxHealth, currentHealthBonuses, currentHealthMultipliers));

        [SerializeField]
        private List<float> currentManaBonuses = new();
        private List<float> currentManaMultipliers = new();
        public int MaxMana => Mathf.RoundToInt(CalculateBonuses(ManagerHolder.instance.manaManager.MaxMana, currentManaBonuses, currentManaMultipliers));

        [SerializeField] private int defense;
        private List<float> currentDefenseBonuses = new();
        private List<float> currentDefenseMultipliers = new();
        public int Defense => Mathf.RoundToInt(CalculateBonuses(defense, currentDefenseBonuses, currentDefenseMultipliers));

        [SerializeField] [LabelText("Crit Chance Bonus (%)")]
        private int critChanceBonus;
        private List<float> currentCritChanceBonuses = new();
        private List<float> currentCritChanceMultipliers = new();
        public int CritChanceBonus => Mathf.RoundToInt(CalculateBonuses(critChanceBonus, currentCritChanceBonuses, currentCritChanceMultipliers));

        [SerializeField] [LabelText("Melee Damage Multiplier")]
        private float meleeDamageMult = 1;
        private List<float> currentMeleeDamageBonuses = new();
        private List<float> currentMeleeDamageMultipliers = new();
        public float MeleeDamageMult => CalculateBonuses(meleeDamageMult, currentMeleeDamageBonuses, currentMeleeDamageMultipliers);

        [SerializeField] [LabelText("Range Damage Multiplier")]
        private float rangeDamageMult = 1;
        private List<float> currentRangeDamageBonuses = new();
        private List<float> currentRangeDamageMultipliers = new();
        public float RangeDamageMult => CalculateBonuses(rangeDamageMult, currentRangeDamageBonuses, currentRangeDamageMultipliers);

        [SerializeField] [LabelText("Magic Damage Multiplier")]
        private float magicDamageMult = 1;
        private List<float> currentMagicDamageBonuses = new();
        private List<float> currentMagicDamageMultipliers = new();
        public float MagicDamageMult => CalculateBonuses(magicDamageMult, currentMagicDamageBonuses, currentMagicDamageMultipliers);

        [SerializeField] [LabelText("Attack Speed Multiplier")]
        private float attackSpeedMult = 1;
        private List<float> currentAttackSpeedBonuses = new();
        private List<float> currentAttackSpeedMultipliers = new();
        public float AttackSpeedMult => CalculateBonuses(attackSpeedMult, currentAttackSpeedBonuses, currentAttackSpeedMultipliers);

        [SerializeField] private float moveSpeedBonus;
        private List<float> currentMoveSpeedBonuses = new();
        private List<float> currentMoveSpeedMultipliers = new();
        public float MoveSpeedBonus => CalculateBonuses(moveSpeedBonus, currentMoveSpeedBonuses, currentMoveSpeedMultipliers);
        
        public static readonly HashSet<ItemSO.StatToChange> percentageStatChange = new() { ItemSO.StatToChange.meleeDamageMult, ItemSO.StatToChange.rangeDamageMult, ItemSO.StatToChange.magicDamageMult };
        
        public void ApplyBuff(ItemSO.StatToChange statToChange, float value, float duration, bool isMult) {
            Debug.Log("In ApplyBuff: " + statToChange + " " + value + " " + duration + " " + isMult);
            StartCoroutine(StartBuff(statToChange, value, duration, isMult));
        }

        private IEnumerator StartBuff(ItemSO.StatToChange statToChange, float value, float duration, bool isMult) {
            List<float> bonusList = null;
            List<float> multList = null;
            
            Debug.Log("At the start of coroutine: " + statToChange + " " + value + " " + duration + " " + isMult);
            
            // SEARCHES FOR CORRECT LIST
            switch (statToChange) {
                case ItemSO.StatToChange.maxHealth:
                    Debug.Log(value);
                    if (isMult == true) multList = currentHealthMultipliers;
                    else bonusList = currentHealthBonuses;
                    break;
                
                case ItemSO.StatToChange.maxMana:
                    if (isMult == true) multList = currentManaMultipliers;
                    else bonusList = currentManaBonuses;
                    break;
                
                case ItemSO.StatToChange.defense:
                    if (isMult == true) multList = currentDefenseMultipliers;
                    else bonusList = currentDefenseBonuses;
                    break;
                
                case ItemSO.StatToChange.critChanceBonus:
                    if (isMult == true) multList = currentCritChanceMultipliers;
                    else bonusList = currentCritChanceBonuses;
                    break;
                
                case ItemSO.StatToChange.meleeDamageMult:
                    if (isMult == true) multList = currentMeleeDamageMultipliers;
                    else {
                        value /= 100; // e.g. 5(%) = 0.05
                        bonusList = currentMeleeDamageBonuses;
                    }
                    break;
                
                case ItemSO.StatToChange.rangeDamageMult:
                    if (isMult == true) multList = currentRangeDamageMultipliers;
                    else {
                        value /= 100; // e.g. 5(%) = 0.05
                        bonusList = currentRangeDamageBonuses;
                    }
                    break;
                
                case ItemSO.StatToChange.magicDamageMult:
                    if (isMult == true) multList = currentMagicDamageMultipliers;
                    else {
                        value /= 100; // e.g. 5(%) = 0.05
                        bonusList = currentMagicDamageBonuses;
                    }
                    break;
                
                case ItemSO.StatToChange.attackSpeedMult:
                    if (isMult == true) multList = currentAttackSpeedMultipliers;
                    else {
                        value /= 100; // e.g. 5(%) = 0.05
                        bonusList = currentAttackSpeedBonuses;
                    }
                    break;
                
                case ItemSO.StatToChange.moveSpeedBonus:
                    if (isMult == true) multList = currentMoveSpeedMultipliers;
                    else bonusList = currentMoveSpeedBonuses;
                    break;
            }

            if (bonusList != null) {
                bonusList.Add(value);
                Debug.Log("After adding to bonus list");
            } else {
                multList.Add(value);
                Debug.Log("After adding to mult list: " + multList[0] + " " + currentMeleeDamageMultipliers[0]);
            }
            
            
            UpdateHealthAndMana(statToChange);

            yield return new WaitForSeconds(duration);
            
            if (bonusList != null) {
                bonusList.Remove(Mathf.RoundToInt(value));
            } else {
                multList.Remove(value);
            }
            
            UpdateHealthAndMana(statToChange);
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

        private float CalculateBonuses(float stat, List<float> bonuses, List<float> multipliers) {
            
            foreach (float bonus in bonuses) {
                stat += bonus;
            }

            foreach (float mult in multipliers) {
                stat *= mult;
            }

            return stat;
        }

#if UNITY_EDITOR
        private void Update() {
            if(Input.GetKeyDown(KeyCode.L))
                Debug.Log($"Max Health: {ManagerHolder.instance.healthManager.MaxHealthAfterBonus}\n" +
                          $"Max Mana: {ManagerHolder.instance.manaManager.MaxManaAfterBonus}\n" +
                          $"Defense: {ManagerHolder.instance.statsManager.Defense}\n" +
                          $"Crit Chance Bonus: {ManagerHolder.instance.statsManager.CritChanceBonus}\n" +
                          $"Melee Damage Mult: {ManagerHolder.instance.statsManager.MeleeDamageMult}\n" +
                          $"Range Damage Mult: {ManagerHolder.instance.statsManager.RangeDamageMult}\n" +
                          $"Magic Damage Mult: {ManagerHolder.instance.statsManager.MagicDamageMult}\n" +
                          $"Attack Speed Mult: {ManagerHolder.instance.statsManager.AttackSpeedMult}\n" +
                          $"Move Speed Bonus: {ManagerHolder.instance.statsManager.MoveSpeedBonus}");
        }
#endif
    }
}