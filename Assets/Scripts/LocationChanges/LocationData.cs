#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
#endif

[System.Serializable]
public class LocationData
{
    public LocationId id;

    public SceneAsset scene; // видно в инспекторе, но в билде не используется

    public Sprite detailedMap;

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
