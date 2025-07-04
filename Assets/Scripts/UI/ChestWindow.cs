using System.Collections.Generic;
using UnityEngine;

public class ChestWindow : UIWindow
{
    [SerializeField] private Transform slotContainer;
    [SerializeField] private InventorySlot slotPrefab;
    [SerializeField] private InventoryWindow inventoryWindow;

    private Chest currentChest;
    private List<InventorySlot> slots = new List<InventorySlot>();

    public void Open(Chest chest)
    {
        currentChest = chest;
        gameObject.SetActive(true);
        UpdateUI();
        inventoryWindow.UpdateUI();
    }

    public override void Hide()
    {
        gameObject.SetActive(false);
        currentChest = null;
    }

    private void UpdateUI()
    {
        if (currentChest == null)
        {
            return;
        }
        var items = currentChest.GetItems();

        // Обновление слотов
        while (slots.Count < items.Count)
        {
            var slot = Instantiate(slotPrefab, slotContainer);
            slots.Add(slot);
        }

        for (int i = 0; i < slots.Count; i++)
        {
            if (i < items.Count)
            {
                var invItem = items[i];
                slots[i].AddItem(invItem.item, invItem.quantity);
                slots[i].SlotID = i;

                // Заменяем поведение кнопки
                slots[i].IconButton.onClick.RemoveAllListeners();
                slots[i].IconButton.onClick.AddListener(() =>
                {
                    Inventory.instance.Add(invItem.item, invItem.quantity);
                    currentChest.RemoveItem(invItem, invItem.quantity);
                    UpdateUI();
                    inventoryWindow.UpdateUI();
                });
            }
            else
            {
                slots[i].ClearSlot();
            }
        }
    }

    public override void Show()
    {
        throw new System.NotImplementedException();
    }
}

