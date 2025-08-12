using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [HideInInspector] public int SlotID = 0;

    public Image icon;
    public Button IconButton;
    public TextMeshProUGUI stackText;

    private Item item;
    private int quantity;

    // Drag helper
    private GameObject dragIcon;
    private RectTransform dragTransform;
    private Canvas canvas;

    private void Awake()
    {
        canvas = GetComponentInParent<Canvas>();
    }

    public void AddItem(Item newItem, int count)
    {
        item = newItem;
        quantity = count;
        if (item.icon != null)
        {
            icon.sprite = item.icon;
        }
        icon.enabled = true;
        IconButton.interactable = true;
        UpdateStackText();
    }

    public void ClearSlot()
    {
        item = null;
        quantity = 0;

        icon.sprite = null;
        icon.enabled = false;
        IconButton.interactable = false;
        stackText.text = "";
    }

    private void UpdateStackText()
    {
        stackText.text = quantity > 1 ? quantity.ToString() : "";
    }

    public Item GetItem() => item;

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (item == null || icon.sprite == null) return;

        dragIcon = new GameObject("DragIcon");
        dragIcon.transform.SetParent(canvas.transform, false);
        dragIcon.transform.SetAsLastSibling();

        dragTransform = dragIcon.AddComponent<RectTransform>();
        dragTransform.sizeDelta = new Vector2(40, 40); // Можно настроить размер

        var image = dragIcon.AddComponent<Image>();
        image.sprite = icon.sprite;
        image.raycastTarget = false;

        UpdateDragIconPosition(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (dragIcon != null)
            UpdateDragIconPosition(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (dragIcon != null)
            Destroy(dragIcon);
    }

    private void UpdateDragIconPosition(PointerEventData eventData)
    {
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform, eventData.position, canvas.worldCamera, out var pos))
        {
            dragTransform.localPosition = pos;
        }
    }
}
