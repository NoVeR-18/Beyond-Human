using BattleSystem;
using UnityEngine;

[CreateAssetMenu(fileName = "New Skill Scroll", menuName = "Inventory/Skill Scroll")]
public class SkillData : Item
{
    public AbilityData ability; // ref on ability data
    public int skillPointCost = 1; // points required to use this skill

    public SkillData(AbilityData abilityData)
    {
        itemName = abilityData.abilityName;
        icon = abilityData.icon;
        ability = abilityData;
    }

    public override void Use()
    {

    }
}
