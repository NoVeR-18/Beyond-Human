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
        return new InteractableSaveData
        {
            id = GetID(),
            isOpened = this.isOpened, // если используется
            isDestroyed = false, // если применимо
            items = new List<InventoryItem>(this.GetItems()),

            position = transform.position,
            rotation = transform.rotation,
        };
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
