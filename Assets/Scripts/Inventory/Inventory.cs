using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

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
[System.Serializable]
public class InventoryItemSerializable
{
    public string itemName;
    public int quantity;
}
[System.Serializable]
public class InventorySaveData
{
    public List<InventoryItemSerializable> savedItems = new();
}
public class Inventory : MonoBehaviour
{
    public static Inventory Instance;
    public List<InventoryItem> items = new List<InventoryItem>();

    void Awake()
    {
        Instance = this;
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
        InventorySaveData saveData = new();

        foreach (var i in items)
        {
            saveData.savedItems.Add(new InventoryItemSerializable
            {
                itemName = i.item.itemName,
                quantity = i.quantity
            });
        }

        string json = JsonUtility.ToJson(saveData);
        PlayerPrefs.SetString("Inventory", json);
        PlayerPrefs.Save();
    }

    public void LoadInventory()
    {
        items.Clear();

        if (!PlayerPrefs.HasKey("Inventory"))
            return;

        string json = PlayerPrefs.GetString("Inventory");
        InventorySaveData saveData = JsonUtility.FromJson<InventorySaveData>(json);

        foreach (var entry in saveData.savedItems)
        {
            // Асинхронная загрузка — нужно через корутину
            StartCoroutine(LoadItemAsync(entry.itemName, entry.quantity));
        }
    }
    private IEnumerator LoadItemAsync(string itemName, int quantity)
    {
        AsyncOperationHandle<Item> handle = Addressables.LoadAssetAsync<Item>(itemName);
        yield return handle;

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            Item loadedItem = handle.Result;
            items.Add(new InventoryItem(loadedItem, quantity));
        }
        else
        {
            Debug.LogWarning($"Не удалось загрузить предмет через Addressables: {itemName}");
        }
    }
    private void OnApplicationQuit()
    {
        SaveInventory();
    }
}
