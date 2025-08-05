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
        icon.sprite = skill.icon;
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
            Inventory.instance.Add(skill);
            ClearSkill();
            PartyWindow.UpdateItems?.Invoke();
        }

    }
}
