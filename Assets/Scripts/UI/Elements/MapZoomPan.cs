using UnityEngine;
using UnityEngine.EventSystems;

public class MapZoomPan : MonoBehaviour, IPointerDownHandler, IDragHandler, IScrollHandler
{
    [Tooltip("RectTransform изображения подробной карты (тот же, который в LocationDetailWindow.detailedMapImage)")]
    [SerializeField] private RectTransform content;

    [Header("Zoom")]
    public float minScale = 0.5f;
    public float maxScale = 3f;
    public float wheelZoomSpeed = 0.1f;

    private Vector2 lastLocalPointer;

    public void ResetView()
    {
        if (content == null) return;
        content.localScale = Vector3.one;
        content.anchoredPosition = Vector2.zero;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)transform, eventData.position, eventData.pressEventCamera, out lastLocalPointer);
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 localPointer;
        RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)transform, eventData.position, eventData.pressEventCamera, out localPointer);
        Vector2 delta = localPointer - lastLocalPointer;
        content.anchoredPosition += delta;
        lastLocalPointer = localPointer;
    }

    public void OnScroll(PointerEventData eventData)
    {
        float delta = eventData.scrollDelta.y;
        float scale = content.localScale.x * (1f + delta * wheelZoomSpeed);
        scale = Mathf.Clamp(scale, minScale, maxScale);
        content.localScale = Vector3.one * scale;
    }
}
