using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LocationDetailWindow : MonoBehaviour
{
    [SerializeField] private GameObject content;
    [SerializeField] private Image detailedMapImage;    // картинка подробной карты (RectTransform = content image)
    [SerializeField] private Button closeButton;
    [SerializeField] private TextMeshProUGUI titleText;            // опционально показать имя локации
    [SerializeField] private MapZoomPan zoomPan;        // компонент для пан/зум (см. ниже)


    public void ShowForEntry(LocationData entry)
    {
        if (detailedMapImage != null && entry.detailedMap != null)
        {
            detailedMapImage.sprite = entry.detailedMap;
            // убедимся, что preserveAspect настроено как нужно
        }
        if (titleText != null) titleText.text = entry.id.ToString();

        content.SetActive(true);
        zoomPan?.ResetView();

        closeButton.onClick.RemoveAllListeners();
        closeButton.onClick.AddListener(Hide);
    }

    public void Hide()
    {
        content.SetActive(false);
    }
}
