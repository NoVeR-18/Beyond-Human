using System;
using UnityEngine;
using UnityEngine.UI;

public class PartyCharacterPanel : MonoBehaviour
{
    [Header("Common")]
    [SerializeField] private Image portrait;

    [Header("Gear UI")]
    [SerializeField] private GameObject gearRoot;
    [SerializeField] private Transform equipmentSlotContainer; // 8 слотов: Helmet, Chest и т.д.
    [SerializeField] private EquipmentSlotUI equipmentSlotPrefab;

    [Header("Weapon UI")]
    [SerializeField] private WeaponSlotUI mainHand;
    [SerializeField] private WeaponSlotUI offHand;

    [Header("Skill UI")]
    [SerializeField] private GameObject skillsRoot;
    [SerializeField] private EquippedSkillsBook equippedSkillsUI;
    [SerializeField]
    private Character character;

    public void SetData(Character data)
    {
        character = data;
        portrait.sprite = character.portrait;
        ShowGear();
        ShowSkills();
    }

    public void ShowGear()
    {
        // Очистить старые и отрисовать текущие
        foreach (Transform child in equipmentSlotContainer)
            Destroy(child.gameObject);

        foreach (EquipmentSlot slot in Enum.GetValues(typeof(EquipmentSlot)))
        {
            var uiSlot = Instantiate(equipmentSlotPrefab, equipmentSlotContainer);
            uiSlot.SetSlot(slot, character);
        }

        mainHand.SetWeapon(character.weaponMainHand, character);
        offHand.SetWeapon(character.weaponOffHand, character, false);
    }

    public void ShowSkills()
    {
        equippedSkillsUI.SetSkills(character.equippedSkills);
    }
    public Character GetCharacter() => character;
}
