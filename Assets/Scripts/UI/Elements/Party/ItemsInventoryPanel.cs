using UnityEngine;

public class ItemsInventoryPanel : MonoBehaviour
{
    [SerializeField] private Transform content;
    [SerializeField] private InventorySlotUI slotPrefab;

    private void OnEnable()
    {
        UpdateUI();
    }

    public void UpdateUI()
    {
        foreach (Transform child in content)
            Destroy(child.gameObject);

        foreach (var entry in Inventory.instance.items)
        {
            if (entry.item is Weapon weapon)
            {
                var slot = Instantiate(slotPrefab, content);
                slot.Set(weapon, Inventory.instance.GetItemCount(entry));
            }
            else if (entry.item is Equipment equipment)
            {
                var slot = Instantiate(slotPrefab, content);
                slot.Set(equipment, Inventory.instance.GetItemCount(entry));
            }
        }
    }
}
