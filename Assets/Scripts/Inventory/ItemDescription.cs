using HealthAndStats;
using Player;
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
        [SerializeField] private RectTransform panel;
        [SerializeField] private Canvas canvas;

        private void Update() {
            panel.gameObject.SetActive(InventoryManager.instance.CanDisplayDescription());
            
            transform.position = PlayerController.instance.MousePos;
            ManagePivot();
        }

        private void ManagePivot() {
            Vector2 screenPos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.transform as RectTransform,
                PlayerController.instance.MousePos,
                canvas.worldCamera,
                out screenPos
            );

            float pivotWorldOffsetX = 1f / panel.rect.width;
            float pivotWorldOffsetY = 1f / panel.rect.height;

            float pivotX = 1 - Mathf.Clamp(pivotWorldOffsetX * (Screen.width/2f - screenPos.x), 0, 1);
            float pivotY = Mathf.Clamp(pivotWorldOffsetY * (Screen.height/2f + screenPos.y), 0, 1);
            
            pivotX = Screen.width/2f - screenPos.x < panel.rect.width ? pivotX : 0;
            pivotY = Screen.height/2f + screenPos.y < panel.rect.height ? pivotY : 1;
            
            panel.pivot = new Vector2(pivotX, pivotY);
        }

        public void UpdateDescription(ItemSO itemSO) {
            itemName.text = itemSO.itemName;
            itemName.color = InventoryManager.rarityColors[itemSO.rarity.ToString()];

            string descriptionText = "";
            
            
            switch (itemSO) {
                case ItemConsumable item:
                    foreach (ItemConsumable.ModifyStat modifyStat in item.statsToModify) {
                        string statString = SplitStatName(modifyStat.stat.ToString(), true);
                        string value = ValueToText(modifyStat.value, modifyStat.stat, false);

                        if (modifyStat.isBuff) {
                            string modifierText = modifyStat.value < 0 ? "decreased" : "increased";
                            descriptionText += $"{statString} is {modifierText} by {value} for {modifyStat.buffDuration}s\n";
                        } else {
                            string modifierText = modifyStat.value < 0 ? "Lose" : "Restore";
                            descriptionText += $"{modifierText} {value} {statString}\n";
                        }
                            
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
                        string value = ValueToText(modifyStat.value, modifyStat.stat);
                        
                        descriptionText += $"{value} {statString}\n";
                    }

                    break;
            }

            description.text = descriptionText;
        }

        private string SplitStatName(string statName, bool deleteMax = false) {
            statName = System.Text.RegularExpressions.Regex.Replace(statName, "(\\B[A-Z])", " $1");

            if (deleteMax == true) statName = statName.Replace("Max ", "");
            
            return statName;
        }

        private string ValueToText(float value, ItemSO.StatToChange stat, bool addSign = true) {
            string valueText = value >= 0 ? "+" + value : value.ToString();
            
            if (addSign == false) valueText = valueText.Replace("-", "").Replace("+", "");
            
            if (StatsManager.percentageStatChange.Contains(stat)) valueText += "%";

            return valueText;
        }
    }
}