using GameWorld; // чтобы видеть LocationId и LocationManager
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class MapLocationButton : MonoBehaviour
{
    [SerializeField] private LocationId locationId;
    [SerializeField] private string customLabel; // если захочешь подписать

    private void Awake()
    {
        var btn = GetComponent<Button>();
        btn.onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        Debug.Log($"Клик по локации: {locationId}");

        // сохраняем выбранную локацию
        LocationManager.Instance.SetCurrentLocation(locationId);

        // загружаем сцену этой локации
        var scene = LocationManager.Instance.worldDatabase.GetScene(locationId);
        if (!string.IsNullOrEmpty(scene))
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(scene);
        }
    }
}
