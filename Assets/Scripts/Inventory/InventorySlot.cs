using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    [HideInInspector]
    public int SlotID = 0;
    public Image icon;
    public Button removeButton;
    public TextMeshProUGUI stackText;

    private Item item;
    private int quantity;
    private void Start()
    {
        removeButton.onClick.AddListener(() => { ClearSlot(); });


    }
    public void AddItem(Item newItem, int count)
    {
        item = newItem;
        quantity = count;

        icon.sprite = item.icon;
        icon.enabled = true;
        removeButton.interactable = true;
        UpdateStackText();
    }

    public void ClearSlot()
    {
        item = null;
        quantity = 0;

        icon.sprite = null;
        icon.enabled = false;
        removeButton.interactable = false;
        stackText.text = "";
    }



    private void UpdateStackText()
    {
        stackText.text = quantity > 1 ? quantity.ToString() : "";
    }
}
