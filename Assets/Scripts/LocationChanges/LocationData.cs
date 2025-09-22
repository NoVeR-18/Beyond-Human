#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class LocationData
{
    public LocationId id;

#if UNITY_EDITOR
    public SceneAsset scene; // видно в инспекторе, но в билде не используется
#endif


    public string SceneName => scene.name;

#if UNITY_EDITOR
    // Автоматически обновляем sceneName при изменении SceneAsset
    public void OnValidate()
    {
        if (scene != null)
        {
            string path = AssetDatabase.GetAssetPath(scene);
        }
    }
#endif
}
