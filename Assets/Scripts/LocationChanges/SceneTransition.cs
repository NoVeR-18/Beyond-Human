using GameWorld;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Collider2D))]
public class SceneTransition : MonoBehaviour
{
    [Header("Transition Settings")]
    public LocationId targetLocation;     // в какую локацию идём
    public string targetEntrance;     // точка входа в новой сцене

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        // сохраняем текущую сцену
        LocationManager.Instance.SetCurrentLocation(targetLocation);

        // сохраняем "куда встать" после загрузки
        PlayerPrefs.SetString("NextEntrance", targetEntrance);
        PlayerPrefs.Save();

        // грузим сцену
        var scene = LocationManager.Instance.worldDatabase.GetScene(targetLocation);
        if (scene != null)
            SceneManager.LoadScene(scene);
    }
}
