using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InventoryItem
{
    public Item item;
    public int quantity;

    public InventoryItem(Item newItem, int amount)
    {
        item = newItem;
        quantity = amount;
    }
}

public class Inventory : MonoBehaviour
{
    public static Inventory instance;
    public List<InventoryItem> items = new List<InventoryItem>();
    public InventoryWindow inventoryUI;

    void Awake()
    {
        instance = this;
        LoadInventory();
    }

    public void Add(Item item, int amount = 1)
    {
        if (!item.showInInventory) return;

        int remainingAmount = amount;

        // Заполняем уже существующие неполные стеки
        foreach (var inventoryItem in items)
        {
            if (inventoryItem.item == item && inventoryItem.quantity < item.maxStack)
            {
                int addable = Mathf.Min(item.maxStack - inventoryItem.quantity, remainingAmount);
                inventoryItem.quantity += addable;
                remainingAmount -= addable;

                if (remainingAmount <= 0)
                    break;
            }
        }

        // Если ещё остались предметы и есть место, создаём новые стеки
        while (remainingAmount > 0)
        {
            int stackSize = Mathf.Min(remainingAmount, item.maxStack);
            items.Add(new InventoryItem(item, stackSize));
            remainingAmount -= stackSize;
        }

        if (remainingAmount > 0)
        {
            Debug.Log("Инвентарь переполнен!");
        }

        SaveInventory();
    }

    public void Remove(Item item, int amount = 1)
    {
        InventoryItem existingItem = items.Find(i => i.item == item);
        if (existingItem != null)
        {
            existingItem.quantity -= amount;
            if (existingItem.quantity <= 0)
                items.Remove(existingItem);
        }
        SaveInventory();
    }


    public int GetItemCount(InventoryItem item)
    {
        return item.quantity;
    }
    public void SaveInventory()
    {
        PlayerPrefs.SetString("Inventory", JsonUtility.ToJson(this));
        PlayerPrefs.Save();
    }

    public void LoadInventory()
    {
        if (PlayerPrefs.HasKey("Inventory"))
        {
            JsonUtility.FromJsonOverwrite(PlayerPrefs.GetString("Inventory"), this);
        }
    }
}
