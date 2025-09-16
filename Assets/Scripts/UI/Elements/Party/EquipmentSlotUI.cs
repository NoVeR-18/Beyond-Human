using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EquipmentSlotUI : MonoBehaviour, IDropHandler, IPointerClickHandler
{
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI slotLabel;


    public EquipmentSlot slotType;
    private Character ownerCharacter;
    private Equipment equipped;

    public void SetSlot(EquipmentSlot slot, Character character)
    {
        slotType = slot;
        ownerCharacter = character;

        equipped = character.GetEquippedBySlot(slot);
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (equipped != null)
        {
            icon.sprite = equipped.icon;
            icon.color = new Color(1f, 1f, 1f, 1f);
        }
        else
        {
            icon.sprite = null;
            icon.color = new Color(1f, 1f, 1f, 0f);
        }

        slotLabel.text = slotType.ToString();
    }

    public void OnDrop(PointerEventData eventData)
    {
        var drag = DragManager.instance.GetDraggedItem();
        if (drag != null && drag is Equipment eq && eq.equipSlot == slotType)
        {
            if (equipped != null)
            {
                Inventory.Instance.Add(equipped, 1);
            }
            ownerCharacter.Equip(eq);
            PartyManager.Instance.SyncEquipmentWithCharacter(ownerCharacter);
            Inventory.Instance.Remove(eq, 1);
            equipped = eq;
            UpdateUI();

        }
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (equipped != null)
        {
            var character = GetComponentInParent<PartyCharacterPanel>().GetCharacter();
            character.UnequipItem(equipped);
            Inventory.Instance.Add(equipped);
            equipped = null;
            UpdateUI();

            PartyWindow.UpdateItems?.Invoke();
        }
    }
}
