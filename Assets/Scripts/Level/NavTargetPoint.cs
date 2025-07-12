using UnityEngine;
[System.Serializable]
public class NavTargetPoint : MonoBehaviour
{
    public HouseData house;      // null если это улица
    public int floorIndex = 0;   // игнорируется, если house == null

    public bool IsInsideHouse => house != null;

    [SerializeField] private string pointId;

    public string PointId => pointId;

    private void Awake()
    {
        SaveSystem.Instance.points.Add(pointId, this);
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (string.IsNullOrEmpty(pointId))
        {
            pointId = $"{gameObject.scene.name}_{gameObject.name}_{System.Guid.NewGuid().ToString().Substring(0, 8)}";
        }
    }
#endif
}