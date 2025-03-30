using Sirenix.OdinInspector;
using UnityEngine;

public class StatsManager : MonoBehaviour {
    [InfoBox("Leave all at 0 for player")]
    [SerializeField]
    private int maxHealthBonus;
    public int MaxHealthBonus => maxHealthBonus;
    
    [SerializeField]
    private int defense;
    public int Defense => defense;
    
    [SerializeField]
    private int critChanceBonus;
    public int CritChanceBonus => critChanceBonus;
    
    [SerializeField]
    [LabelText("Melee Damage Bonus (%)")]
    private int meleeDamageBonus;
    public int MeleeDamageBonus => meleeDamageBonus;
    
    [SerializeField]
    [LabelText("Range Damage Bonus (%)")] private int rangeDamageBonus;
    public int RangeDamageBonus => rangeDamageBonus;
    
    [SerializeField]
    [LabelText("Magic Damage Bonus (%)")]
    private int magicDamageBonus;
    public int MagicDamageBonus => magicDamageBonus;
    
    [SerializeField]
    [LabelText("Attack Speed Bonus (%)")]
    private int attackSpeedBonus;
    public int AttackSpeedBonus => attackSpeedBonus;
    
    [SerializeField]
    private int moveSpeedBonus;
    public int MoveSpeedBonus => moveSpeedBonus;
    
    private void Start()
    {
        
    }

    private void Update()
    {
        
    }
}
