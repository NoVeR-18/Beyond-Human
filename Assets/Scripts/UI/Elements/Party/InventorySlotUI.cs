using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlotUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI stackText;

    private Item skillData;

    public void Set(Item data, int count)
    {
        skillData = data;

        icon.sprite = skillData.icon;
        nameText.text = skillData.itemName;
        stackText.text = count > 1 ? $"x{count}" : "";
        icon.enabled = true;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        DragManager.instance.BeginDrag(skillData, icon.sprite);
    }

    public void OnDrag(PointerEventData eventData)
    {
        DragManager.instance.UpdateDrag(eventData.position);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        DragManager.instance.EndDrag();
    }

    public Item GetSkill() => skillData;
}
