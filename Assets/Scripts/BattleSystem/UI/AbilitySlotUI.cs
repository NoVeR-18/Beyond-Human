using UnityEngine;
using UnityEngine.UI;

namespace BattleSystem
{
    public class AbilitySlotUI : MonoBehaviour
    {
        [SerializeField] private Image icon;
        [SerializeField] private Image cooldownOverlay;
        [SerializeField]
        private AbilityData ability;

        public void Init(AbilityData abilityData)
        {
            ability = abilityData;
            icon.sprite = ability.icon;
            cooldownOverlay.fillAmount = 0;
        }

        public void UpdateCooldown(float remaining, float max)
        {
            if (remaining > 0)
            {
                cooldownOverlay.fillAmount = remaining / max;
            }
            else
            {
                cooldownOverlay.fillAmount = 0;
            }
        }


        public AbilityData GetAbility() => ability;
    }

}
