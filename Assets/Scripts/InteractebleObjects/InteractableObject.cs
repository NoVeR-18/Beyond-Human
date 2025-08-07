using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public abstract class InteractableObject : MonoBehaviour, ISaveableInteractable
{
    [SerializeField] private string objectId;

    public string GetID() => objectId;

    protected virtual void Awake()
    {
        SaveSystem.Instance?.RegisterInteractable(this);
    }

    // –едакторска€ генераци€ уникального ID (аналогично NPC)
#if UNITY_EDITOR
    [ContextMenu("Regenerate ID")]
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
        }
    }

    private string GenerateId()
    {
        return $"{gameObject.scene.name}_{gameObject.name}_{Guid.NewGuid().ToString().Substring(0, 8)}";
    }
#endif

    public abstract InteractableSaveData GetSaveData();
    public abstract void LoadFromData(InteractableSaveData data);

    public abstract void Destroy();
}
