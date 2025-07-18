using System;
using UnityEditor;
using UnityEngine;
[System.Serializable]
public class NavTargetPoint : MonoBehaviour
{
    public HouseData house;      // null если это улица
    public int floorIndex = 0;   // игнорируется, если house == null

    public bool IsInsideHouse => house != null;

    public string pointId;


    private void Awake()
    {
        SaveSystem.Instance.points.Add(pointId, this);
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (!Application.isPlaying && gameObject.scene.IsValid())
        {
            // Если в сцене уже есть другой объект с таким же ID — перегенерируем
            var npcs = GameObject.FindObjectsOfType<NavTargetPoint>();
            bool duplicate = false;

            foreach (var other in npcs)
            {
                if (other != this && other.pointId == this.pointId)
                {
                    duplicate = true;
                    break;
                }
            }

            if (string.IsNullOrEmpty(pointId) || duplicate)
            {
                pointId = GenerateId();
                EditorUtility.SetDirty(this);
            }
        }
    }

    private string GenerateId()
    {
        return $"{gameObject.scene.name}_{gameObject.name}_{Guid.NewGuid().ToString().Substring(0, 8)}";
    }
#endif
}