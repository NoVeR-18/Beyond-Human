using BattleSystem;
using UnityEngine;

[CreateAssetMenu(fileName = "New Skill Scroll", menuName = "Inventory/Skill Scroll")]
public class SkillData : Item
{
    public AbilityData ability; // ref on ability data
    public int skillPointCost = 1; // points required to use this skill

    public override void Use()
    {

    }
}
