using Assets.Scripts.NPC;
using BattleSystem;
using UnityEngine;


[CreateAssetMenu(menuName = "Dialogue/Actions/Hire Companion")]
public class DialogueHireCompanionAction : DialogueAction
{
    public Character HiredCharacter;
    public EquipedItems equipedItems;
    public CharacterStats stats;
    public override void Execute(NPCController npc, PlayerController player)
    {
        HiredCharacter.battleCharacter.CurrentStats = stats;
        HiredCharacter.equippedItems = new()
        {
            [EquipmentSlot.Helmet] = equipedItems.helmet,
            [EquipmentSlot.Bib] = equipedItems.chest,
            [EquipmentSlot.Pants] = equipedItems.pants,
            [EquipmentSlot.Shoes] = equipedItems.shoes,
            [EquipmentSlot.Gloves] = equipedItems.gloves,
            [EquipmentSlot.Amulet] = equipedItems.amulet
        };
        HiredCharacter.battleCharacter.Team = BattleTeam.Team1;
        PartyManager.Instance.AddMember(HiredCharacter);
    }
}
[System.Serializable]
public class EquipedItems
{
    public Equipment helmet;
    public Equipment chest;
    public Equipment pants;
    public Equipment shoes;
    public Equipment gloves;
    public Equipment amulet;
}