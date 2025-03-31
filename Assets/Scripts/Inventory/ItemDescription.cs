using System;
using System.Collections.Generic;
using System.Diagnostics;
using HealthAndStats;
using TMPro;
using UnityEngine;

namespace Inventory {
    public class ItemDescription : MonoBehaviour {
        public static ItemDescription instance;

        private void Awake() {
            if (instance == null) { instance = this; } else { Destroy(gameObject); }
        }

        [SerializeField] private TMP_Text itemName;
        [SerializeField] private TMP_Text description;
        [SerializeField] private GameObject panel;

        private void Update() {
            panel.SetActive(InventoryManager.instance.CanDisplayDescription());

            transform.position = Input.mousePosition;
        }

        public void UpdateDescription(ItemSO itemSO) {
            itemName.text = itemSO.itemName;
            itemName.color = InventoryManager.rarityColors[itemSO.rarity.ToString()];

            string descriptionText = "";

            switch (itemSO) {
                case ItemConsumable item:
                    foreach (ItemConsumable.ModifyStat modifyStat in item.statsToModify) {
                        string statString = SplitStatName(modifyStat.stat.ToString());

                        descriptionText += $"Restores {statString} by {modifyStat.value}\n";
                    }

                    break;

                case ItemWeapon item:
                    descriptionText += $"{item.damageValue} {item.damageType} damage\n" +
                                       $"{item.attackSpeed} attacks per second\n" +
                                       $"{item.critChance}% critical strike\n";
                    break;

                case ItemArmor item:
                    foreach (ItemArmor.ModifyStat modifyStat in item.statsToModify) {
                        string statString = SplitStatName(modifyStat.stat.ToString());

                        if (StatsManager.percentageStatChange.Contains(modifyStat.stat))
                            descriptionText += $"+{modifyStat.value}% {statString}\n";
                        else
                            descriptionText += $"+{modifyStat.value} {statString}\n";
                    }

                    break;
            }

            description.text = descriptionText;
        }

        private string SplitStatName(string statName) {
            return System.Text.RegularExpressions.Regex.Replace(statName, "(\\B[A-Z])", " $1").ToLower();
        }
    }
}