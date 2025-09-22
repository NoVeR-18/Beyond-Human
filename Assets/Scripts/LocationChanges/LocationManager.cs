using UnityEngine;

namespace GameWorld
{
    public class LocationManager : MonoBehaviour
    {
        public static LocationManager Instance { get; private set; }

        public WorldDatabase worldDatabase;
        [SerializeField]
        private LocationId currentLocation = LocationId.None;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
            // загружаем последнюю локацию из PlayerPrefs
            LoadLastLocation();
        }

        public void SetCurrentLocation(LocationId location)
        {
            currentLocation = location;
            PlayerPrefs.SetInt("LastLocation", (int)currentLocation);
            PlayerPrefs.Save();
        }

        public LocationId GetCurrentLocation()
        {
            return currentLocation;
        }

        public void LoadLastLocation()
        {
            if (PlayerPrefs.HasKey("LastLocation"))
            {
                LocationId saved = (LocationId)PlayerPrefs.GetInt("LastLocation");
                var scene = worldDatabase.GetScene(saved);
                if (scene != null)
                {
                    if (scene != UnityEngine.SceneManagement.SceneManager.GetActiveScene().name)
                        UnityEngine.SceneManagement.SceneManager.LoadScene(scene);
                }
            }
            else
            {
                // если нет сохранённого места, грузим дефолт
                var scene = worldDatabase.GetScene(LocationId.Enforcers);
                if (scene != null)
                {
                    if (scene != UnityEngine.SceneManagement.SceneManager.GetActiveScene().name)
                        UnityEngine.SceneManagement.SceneManager.LoadScene(scene);
                }
            }
        }
    }
}
