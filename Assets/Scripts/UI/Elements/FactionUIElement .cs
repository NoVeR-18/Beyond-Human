using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FactionUIElement : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private Slider attitudeSlider;
    [SerializeField] private TextMeshProUGUI attitudeValueText;

    public void Setup(FactionData faction, int attitude)
    {
        if (iconImage != null)
            iconImage.sprite = faction.factionIcon;

        if (nameText != null)
            nameText.text = faction.factionName;

        if (attitudeSlider != null)
        {
            attitudeSlider.minValue = -100;
            attitudeSlider.maxValue = 100;
            attitudeSlider.value = attitude;
        }

        if (attitudeValueText != null)
            attitudeValueText.text = $"{attitude}";
    }
}
