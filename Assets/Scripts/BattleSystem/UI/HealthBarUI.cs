using UnityEngine;
using UnityEngine.UI;

namespace BattleSystem
{
    public class HealthBarUI : MonoBehaviour
    {
        [SerializeField] private Image fillImage;

        public void SetHealth(float current, float max)
        {
            fillImage.fillAmount = current / max;
        }
    }
}