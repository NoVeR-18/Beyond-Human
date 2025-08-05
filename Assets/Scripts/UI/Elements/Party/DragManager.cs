using UnityEngine;
using UnityEngine.UI;

public class DragManager : MonoBehaviour
{
    public static DragManager instance;

    [SerializeField] private Canvas canvas;
    [SerializeField] private Image dragIcon;

    private Item currentItem;

    void Awake()
    {
        instance = this;
        dragIcon.gameObject.SetActive(false);
    }

    public void BeginDrag(Item item, Sprite icon)
    {
        currentItem = item;
        dragIcon.sprite = icon;
        dragIcon.gameObject.SetActive(true);
    }

    public void UpdateDrag(Vector2 position)
    {
        dragIcon.rectTransform.position = position;
    }

    public void EndDrag()
    {
        currentItem = null;
        dragIcon.gameObject.SetActive(false);
        PartyWindow.UpdateItems?.Invoke();
    }

    public SkillData GetDraggedSkill()
    {

        if (currentItem != null && currentItem is SkillData)
            return currentItem as SkillData;
        else
            return null;
    }

    internal Item GetDraggedItem()
    {
        if (currentItem != null && (currentItem is Weapon || currentItem is Equipment))
            return currentItem;
        return null;
    }
}

