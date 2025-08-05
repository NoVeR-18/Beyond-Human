using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EquippedSkillsBook : MonoBehaviour, IDropHandler
{
    [SerializeField] private Transform skillsContainer;
    [SerializeField] private SkillSlotUI skillSlotPrefab;

    public void OnDrop(PointerEventData eventData)
    {
        SkillData draggedSkill = DragManager.instance.GetDraggedSkill();
        if (draggedSkill == null)
            return;

        // Например: добавляем скилл персонажу
        var character = GetComponentInParent<PartyCharacterPanel>().GetCharacter();

        if (character != null && character.CanEquipSkill(draggedSkill))
        {
            character.EquipSkill(draggedSkill);
            SetSkills(draggedSkill);

            Inventory.instance.Remove(draggedSkill, 1);
        }

    }
    public void SetSkills(List<SkillData> skills)
    {
        // Очистка
        foreach (Transform child in skillsContainer)
            Destroy(child.gameObject);

        foreach (var skill in skills)
        {
            SetSkills(skill);
        }
    }
    private void SetSkills(SkillData skill)
    {
        var slot = Instantiate(skillSlotPrefab, skillsContainer);
        slot.SetSkill(skill);

    }
}
