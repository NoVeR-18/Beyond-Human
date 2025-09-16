using UnityEngine;

public class EquipmentSprite : MonoBehaviour
{
    [SerializeField] private EquipmentSlot slotType;
    [SerializeField] private SpriteRenderer spriteRenderer;
    private EquipmentSprites currentSprites;

    public EquipmentSlot SlotType => slotType;

    public void SetEquipment(EquipmentSprites sprites)
    {
        currentSprites = sprites;
    }

    public void UpdateDirection(Direction dir)
    {
        if (currentSprites != null)
            spriteRenderer.sprite = currentSprites.GetSprite(dir);
        else
            spriteRenderer.sprite = null; // если слот пустой
    }
}

