// UIWindow.cs
using UnityEngine;

public abstract class UIWindow : MonoBehaviour
{

    public bool IsVisible => gameObject.activeSelf;
    public abstract void Show();
    public abstract void Hide();
}
