using UnityEngine;

public class TradeWindow : MonoBehaviour
{
    [SerializeField] private GameObject panel;

    public void Show()
    {
        panel.gameObject.SetActive(true);
    }

    public void Hide()
    {
        panel.gameObject.SetActive(false);
    }

    public bool IsVisible => panel.activeSelf;
}
