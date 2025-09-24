using GameWorld;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapWindow : UIWindow
{
    [Header("Main UI")]
    [SerializeField] private GameObject content;              // �������� ������ ����
    [SerializeField] private Image globalMapImage;            // ��� � ���������� ����� (pivot = center)

    public List<MapLocationHotspot> hotspots = new List<MapLocationHotspot>();

    [Header("Detail window")]
    [SerializeField] private LocationDetailWindow detailWindow;

    private void OnEnable()
    {
        // ��������, ��� layout ��� ��������� � ����� ������� ����� �������
        Canvas.ForceUpdateCanvases();
        GenerateHotspots();
    }

    public override void Show() => content.SetActive(true);
    public override void Hide()
    {
        detailWindow.Hide();
        content.SetActive(false);
    }

    private void GenerateHotspots()
    {
        if (LocationManager.Instance == null)
        {
            Debug.LogWarning("LocationManager not found in scene.");
            return;
        }
        var db = LocationManager.Instance.worldDatabase;
        if (db == null)
        {
            Debug.LogWarning("WorldDatabase not assigned on LocationManager.");
            return;
        }

        if (globalMapImage != null && globalMapImage.sprite == null && db.GetWorldMap() != null)
            globalMapImage.sprite = db.GetWorldMap();

        foreach (var hotspot in hotspots)
            hotspot.Setup(() => OnHotspotClicked(db.GetLocation(hotspot.openedLocation)));
    }

    private void OnHotspotClicked(LocationData entry)
    {
        // ��������� ��������� ����� ��� ���� �������
        detailWindow.ShowForEntry(entry);
    }
}
