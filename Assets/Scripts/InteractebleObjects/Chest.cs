using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour, IInteractable
{
    public bool isLocked = false;
    public int requiredLockpickingLevel = 0;
    public bool isOpened = false;
    private Animator animator;

    [SerializeField]
    private List<InventoryItem> items = new List<InventoryItem>();
    public List<InventoryItem> GetItems() => items;

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

    void OpenChest()
    {
        isOpened = true;
        animator.SetTrigger("Open");
        Debug.Log("Chest opened! Rewards received.");
        UIManager.Instance.chestWindow.Open(this);
        // Here you can give a reward to the player
    }
    public void CloseChest()
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
}