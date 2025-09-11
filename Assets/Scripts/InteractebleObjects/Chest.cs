using System.Collections.Generic;
using UnityEngine;

public class Chest : InteractableObject, IInteractable
{
    public bool isLocked = false;
    public int requiredLockpickingLevel = 0;
    public bool isOpened = false;
    private Animator animator;

    [SerializeField]
    protected List<InventoryItem> items = new List<InventoryItem>();
    public List<InventoryItem> GetItems() => items;

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
        objectId = data.id;
        addressableKey = data.prefabId;
        isOpened = data.isOpened;

        transform.position = data.position;
        transform.rotation = data.rotation;

        items = new List<InventoryItem>();

        foreach (var savedItem in data.items)
        {
            LoadItemAsync(savedItem.itemKey, savedItem.quantity);
        }

        if (isOpened)
            animator?.SetTrigger("Open");
        else
            animator?.SetTrigger("Close");
    }

    private async void LoadItemAsync(string key, int quantity)
    {
        var handle = UnityEngine.AddressableAssets.Addressables.LoadAssetAsync<Item>(key);
        await handle.Task;

        if (handle.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
        {
            var loadedItem = handle.Result;
            var invItem = new InventoryItem(loadedItem, quantity);
            items.Add(invItem);
        }
        else
        {
            Debug.LogWarning($"Failed to load Item with key {key}");
        }
    }

    private void Start()
    {
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("Animator component is missing on the chest.");
        }
    }
    public void Interact(PlayerKeysAndSkills player)
    {
        if (isOpened)
        {
            UIManager.Instance.chestWindow.Hide();
            Debug.Log("Chest is opened.");
            return;
        }

        if (isLocked && player.lockpickingLevel < requiredLockpickingLevel)
        {
            Debug.Log("The hack level is too low.");
            return;
        }

        OpenChest();
    }

    public void OpenChest()
    {
        isOpened = true;
        animator.SetTrigger("Open");
        Debug.Log("Chest opened! Rewards received.");
        UIManager.Instance.chestWindow.Open(this);
        // Here you can give a reward to the player
    }
    public virtual void CloseChest()
    {
        isOpened = false;
        animator.SetTrigger("Close");
        Debug.Log("Chest closed!");
    }
    public void RemoveItem(InventoryItem item, int amount = 1)
    {
        item.quantity -= amount;
        if (item.quantity <= 0)
            items.Remove(item);
    }
    public void AddItem(Item item, int amount = 1)
    {
        var existing = items.Find(i => i.item == item);
        if (existing != null)
            existing.quantity += amount;
        else
            items.Add(new InventoryItem(item, amount));
    }

    public override void Destroy()
    {
    }
}