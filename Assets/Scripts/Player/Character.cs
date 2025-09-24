using Assets.Scripts.NPC;
using BattleSystem;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Character
{
    public string characterName;
    public Sprite portrait;
    public Dictionary<EquipmentSlot, Equipment> equippedItems = new(); // Helmet, Chest, Pants, Shoes, Gloves, Amulet
    public Weapon weaponMainHand;
    public Weapon weaponOffHand;
    public BattleCharacter battleCharacter; // Ссылка на BattleCharacter для использования в бою

    public List<SkillData> equippedSkills = new();

    public Character(NPCController npcController = null)
    {
        if (npcController != null)
        {
            characterName = npcController.characterName;
            portrait = npcController.GetComponent<SpriteRenderer>().sprite;
            battleCharacter = npcController.battleParticipantData.battleCharacter;

            if (npcController.battleParticipantData.abilities != null)
            {
                foreach (var ability in npcController.battleParticipantData.abilities)
                {

                    equippedSkills.Add(new SkillData(ability));
                }
            }


        }
    }

    public Equipment GetEquippedBySlot(EquipmentSlot slot)
    {
        if (equippedItems.TryGetValue(slot, out var eq))
            return eq;
        return null;
    }

    public void Equip(Item item)
    {
        if (item != null && item is Equipment)
        {
            var equipment = (Equipment)item;
            equippedItems[equipment.equipSlot] = (Equipment)item;
        }

    }

    public void EquipWeapon(Weapon weapon, bool toMainHand)
    {
        if (IsTwoHanded(weapon))
        {
            weaponMainHand = weapon;
            weaponOffHand = null;
        }
        else
        {
            if (toMainHand)
                weaponMainHand = weapon;
            else
                weaponOffHand = weapon;
        }
    }


    private bool IsTwoHanded(Weapon weapon)
    {
        // В будущем можно добавить флаг `isTwoHanded`
        return weapon != null && weapon.itemName.Contains("2H");
    }

    public bool CanEquipSkill(SkillData draggedSkill)
    {
        // Проверяем, достаточно ли очков навыков
        if (draggedSkill == null)
            return false;
        else
            return true;

    }

    public void EquipSkill(SkillData draggedSkill)
    {
        equippedSkills.Add(draggedSkill);
    }

    public void UnequipSkill(SkillData skill)
    {
        equippedSkills.Remove(skill);
    }
    public void UnequipItem(Item item)
    {
        if (item == null)
            return;
        if (item is Equipment equipment)
        {

            equippedItems[equipment.equipSlot] = null;
        }
        else if (item is Weapon weapon)
        {
            if (weapon == weaponMainHand)
                weaponMainHand = null;
            else if (weapon == weaponOffHand)
                weaponOffHand = null;
        }
    }
}
