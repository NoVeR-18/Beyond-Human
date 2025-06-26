using UnityEngine;
using UnityEngine.UI;

namespace BattleSystem
{


    public class StatusEffectUI : MonoBehaviour
    {
        [SerializeField] private Image iconImage;
        [SerializeField] private Image durationFill;

        private StatusEffect instance;

        public void Init(StatusEffect status)
        {
            instance = status;
            iconImage.sprite = status.Icon;
        }

        public void UpdateCooldown(float remaining)
        {
            if (instance == null) return;

            if (durationFill != null)
                durationFill.fillAmount = remaining / instance.Duration;
        }

        public StatusEffect GetEffect() => instance;
    }

}