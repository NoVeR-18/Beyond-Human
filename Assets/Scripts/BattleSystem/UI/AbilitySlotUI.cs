using UnityEngine;
using UnityEngine.UI;

namespace BattleSystem
{
    public class AbilitySlotUI : MonoBehaviour
    {
        [SerializeField] private Image icon;
        [SerializeField] private Image cooldownOverlay;

        private AbilityData ability;

        public void Init(AbilityData abilityData)
        {
            ability = abilityData;
            icon.sprite = ability.icon;
            cooldownOverlay.fillAmount = 0;
        }

        public void SetCooldown(float current, float max)
        {
            cooldownOverlay.fillAmount = current / max;
        }
    }

}
