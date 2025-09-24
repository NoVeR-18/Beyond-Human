using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class MapLocationHotspot : MonoBehaviour
{
    [SerializeField] private Button button;

    public LocationId openedLocation;
    private Action onClickCallback;

    public void Setup(Action onClick)
    {
        onClickCallback = onClick;
        if (button == null) button = GetComponent<Button>();

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => onClickCallback?.Invoke());
    }
}
