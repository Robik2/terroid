using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Inventory;

namespace Inventory {
    [CreateAssetMenu(fileName = "NewConsumable", menuName = "Scriptable Objects/Items/Item Consumable SO")]
    public class ItemConsumable : ItemSO {
        public List<ModifyStat> statsToModify = new();

        public enum StatToChange {
            health,
            mana,
            magicPower
        };

        [System.Serializable]
        public struct ModifyStat {
            public StatToChange stat;
            public int value;
            public bool isBuff;

            [ShowIf("isBuff")] [LabelText("Buff Duration (s)")]
            public float buffDuration;
        }
    }
}