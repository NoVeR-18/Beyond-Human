using System.Collections.Generic;
using UnityEngine;

public class DestroyingChest : Chest
{
    private bool isDestroyed = false;

    public override void CloseChest()
    {
        base.CloseChest();

        // Проверка при закрытии
        TryDestroyIfEmpty();
    }

    private void TryDestroyIfEmpty()
    {
        if (!isDestroyed && GetItems().Count == 0)
        {
            Destroy();
        }
    }
    public void SetItems(List<InventoryItem> items)
    {
        this.items = items;
    }

    public override void Destroy()
    {
        if (isDestroyed)
        {
            Destroy(gameObject);
        }
        else
        {
            Debug.Log("Droped items");
            isDestroyed = true;
            gameObject.SetActive(false);
            SaveSystem.Instance.MarkAsDestroyed(this);
        }

    }
    public override InteractableSaveData GetSaveData()
    {
        var saveData = new InteractableSaveData
        {
            id = GetID(),
            prefabId = GetPrefabId(),
            isOpened = isOpened,
            position = transform.position,
            rotation = transform.rotation,
            items = new List<InventoryItemSaveData>()
        };

        foreach (var invItem in items)
        {
            saveData.items.Add(new InventoryItemSaveData
            {
                itemKey = invItem.item.itemName,  // используем itemName как ключ Addressables
                quantity = invItem.quantity
            });
        }

        return saveData;
    }

    public override void LoadFromData(InteractableSaveData data)
    {
        if (data.isDestroyed || data.items == null || data.items.Count == 0)
        {
            Destroy(gameObject);
            return;
        }

        base.LoadFromData(data);
    }
}
