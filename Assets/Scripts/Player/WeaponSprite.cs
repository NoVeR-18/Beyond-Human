using UnityEngine;

namespace Assets.Scripts.Player
{
    public class WeaponSprite : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer;
        private EquipmentSprites currentSprites;


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
}
