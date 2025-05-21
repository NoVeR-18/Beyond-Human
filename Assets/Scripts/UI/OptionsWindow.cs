using UnityEngine;

public class OptionsWindow : UIWindow
{
    [SerializeField] private GameObject content;

    public override void Show()
    {
        content.SetActive(true);
    }

    public override void Hide()
    {
        content.SetActive(false);
    }
}
