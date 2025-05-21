using UnityEngine;

public class FractionsWindow : UIWindow
{
    [SerializeField] private GameObject content;

    public override void Show() => content.SetActive(true);
    public override void Hide() => content.SetActive(false);
}
