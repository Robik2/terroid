using Sirenix.OdinInspector;
using UnityEngine;

namespace Inventory {
    public class ItemSO : ScriptableObject {
        public Item itemPrefab;
        public Sprite sprite;
        public string itemName;
        public Rarity rarity;

        [ShowIf("@IsConsumable")] public bool isStackable;
        [ShowIf("isStackable")] public int stackLimit = 99;
        protected bool IsConsumable => this is ItemConsumable;

        public enum Rarity {
            common,
            uncommon,
            rare,
            epic,
            legendary
        }

        public virtual void UseItem() { }
    }
}