using TMPro;
using UnityEngine;

public class SkillPointIndicator : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI pointText;
    [SerializeField] private GameObject circleObject; // UI круг

    public void SetPoints(int remaining)
    {
        pointText.text = remaining.ToString();
        circleObject.SetActive(true); // можно скрывать при нуле
    }
}
