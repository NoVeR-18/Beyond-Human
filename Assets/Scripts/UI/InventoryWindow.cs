using System.Collections.Generic;
using UnityEngine;

public class InventoryWindow : UIWindow
{
    [SerializeField]
    private List<InventorySlot> slots;
    [SerializeField]
    private InventorySlot SlotUIPrefab;
    [SerializeField]
    private Transform SlotContainer;


    private void Awake()
    {
        var itemCount = Inventory.instance.items.Count;
        for (int i = 0; i < itemCount; i++)
        {
            var slot = Instantiate(SlotUIPrefab, SlotContainer);
            slot.icon.sprite = null;
            slot.SlotID = i;
            slots.Add(slot);
        }
    }

    public override void Hide()
    {
        var parrent = GetComponent<Transform>();
        parrent.gameObject.SetActive(false);
    }
    public override void Show()
    {
        var parrent = GetComponent<Transform>();
        parrent.gameObject.SetActive(true);
        UpdateUI();
    }
    public void UpdateUI()
    {
        var items = Inventory.instance.items;
        int itemCount = items.Count;
        if (slots.Count < itemCount)
        {
            // Добавляем недостающие слоты
            for (int i = slots.Count; i < itemCount; i++)
            {
                var slot = Instantiate(SlotUIPrefab, SlotContainer).GetComponent<InventorySlot>();
                slot.icon.sprite = null;
                slot.SlotID = i;
                slots.Add(slot);
            }
        }
        else if (slots.Count > itemCount)
        {
            // Удаляем лишние слоты
            for (int i = slots.Count - 1; i >= itemCount; i--)
            {
                Destroy(slots[i].gameObject);
                slots.RemoveAt(i);
            }
        }
        for (int i = 0; i < slots.Count; i++)
        {
            if (i < itemCount)
            {
                slots[i].AddItem(items[i].item, Inventory.instance.GetItemCount(items[i]));
            }
            else
            {
                slots[i].ClearSlot();
            }
        }
    }
}