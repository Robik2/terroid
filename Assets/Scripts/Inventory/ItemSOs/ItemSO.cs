using Sirenix.OdinInspector;
using UnityEngine;

public class ItemSO : ScriptableObject {
    public Item itemPrefab;
    public Sprite sprite;
    public string itemName;
    
    [ShowIf("@IsConsumable")]
    public bool isStackable;
    
    [ShowIf("isStackable")]
    public int stackLimit = 99;

    protected bool IsConsumable => this is ItemConsumable;

    public virtual void UseItem() { }
}
