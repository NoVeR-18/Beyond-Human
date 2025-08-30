using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillSlotUI : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI nameText;

    private SkillData skill;

    public void SetSkill(SkillData s)
    {
        skill = s;
        if (skill.icon != null)
            icon.sprite = skill.icon;
        else
            icon.sprite = skill.ability.icon; // Fallback icon if none is set
        nameText.text = skill.itemName;
        icon.enabled = true;
    }

    public void ClearSkill()
    {
        Destroy(gameObject);
        //skill = null;
        //icon.sprite = null;
        //icon.enabled = false;
        //nameText.text = "";
    }

    public SkillData GetSkill() => skill;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (skill != null)
        {
            var character = GetComponentInParent<PartyCharacterPanel>().GetCharacter();
            character.UnequipSkill(skill);
            Inventory.Instance.Add(skill);
            ClearSkill();
            PartyWindow.UpdateItems?.Invoke();
        }

    }
}
