using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Assets.Scripts.Level
{
    [RequireComponent(typeof(Light2D))]
    public class WorldLight : MonoBehaviour
    {

        [SerializeField] private Light2D worldLight;

        private TimeManager timeManager;

        [SerializeField] private Gradient lightColorGradient;

        private void Start()
        {
            worldLight = GetComponent<Light2D>();
            timeManager = TimeManager.Instance;
            timeManager.OnTimeChanged += UpdateLightColor;
            worldLight.color = lightColorGradient.Evaluate(percentOfDay(timeManager.CurrentTime));
        }

        private void UpdateLightColor(GameTime time)
        {
            worldLight.color = lightColorGradient.Evaluate(percentOfDay(time));
        }

        private float percentOfDay(GameTime time)
        {
            float totalMinutes = 24 * 60;
            float currentMinutes = time.Hour * 60 + time.Minute;
            return currentMinutes / totalMinutes;
        }
        private void OnDestroy()
        {
            timeManager.OnTimeChanged -= UpdateLightColor;
        }

    }

}
