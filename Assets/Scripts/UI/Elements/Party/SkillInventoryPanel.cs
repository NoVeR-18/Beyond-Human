using UnityEngine;

public class SkillInventoryPanel : MonoBehaviour
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
            if (entry.item is SkillData skillData)
            {
                var slot = Instantiate(slotPrefab, content);
                slot.Set(skillData, Inventory.instance.GetItemCount(entry));
            }
        }
    }
}
