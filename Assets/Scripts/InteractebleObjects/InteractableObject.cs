using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.AddressableAssets;
#endif
public abstract class InteractableObject : MonoBehaviour, ISaveableInteractable
{
    [SerializeField] protected string objectId;
    [SerializeField] protected string addressableKey; // ключ для Addressables
    public string GetID() => objectId;

    protected virtual void Awake()
    {
        SaveSystem.Instance?.RegisterInteractable(this);
    }
    public abstract InteractableSaveData GetSaveData();
    public abstract void LoadFromData(InteractableSaveData data);

    public abstract void Destroy();
    // Редакторская генерация уникального ID (аналогично NPC)
#if UNITY_EDITOR
    [ContextMenu("Regenerate ID & Addressable Key")]
    [Obsolete]
    private void OnValidate()
    {
        if (!Application.isPlaying && gameObject.scene.IsValid())
        {
            var all = GameObject.FindObjectsOfType<InteractableObject>();
            bool duplicate = false;

            foreach (var obj in all)
            {
                if (obj != this && obj.objectId == this.objectId)
                {
                    duplicate = true;
                    break;
                }
            }

            if (string.IsNullOrEmpty(objectId) || duplicate)
            {
                objectId = GenerateId();
                EditorUtility.SetDirty(this);
            }

            if (string.IsNullOrEmpty(addressableKey))
            {
                AssignAddressableKey();
            }
        }
    }

    private string GenerateId()
    {
        return $"{gameObject.scene.name}_{gameObject.name}_{Guid.NewGuid():N}".Substring(0, 16);
    }

    private void AssignAddressableKey()
    {
        var prefab = PrefabUtility.GetCorrespondingObjectFromOriginalSource(gameObject) as GameObject;
        if (prefab != null)
        {
            var path = AssetDatabase.GetAssetPath(prefab);

            // Если уже есть Addressables Settings
            var settings = AddressableAssetSettingsDefaultObject.Settings;
            if (settings != null)
            {
                var entry = settings.CreateOrMoveEntry(AssetDatabase.AssetPathToGUID(path), settings.DefaultGroup);
                entry.address = prefab.name;
                addressableKey = prefab.name;

                EditorUtility.SetDirty(this);
                AssetDatabase.SaveAssets();
                Debug.Log($"[Addressables] Assigned key '{addressableKey}' to {name}");
            }
        }
    }
#endif
    public string GetPrefabId()
    {
        return addressableKey;
    }

}
