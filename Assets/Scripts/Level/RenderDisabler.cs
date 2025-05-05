using UnityEngine;

public class RenderDisabler : MonoBehaviour
{
    Renderer _renderer;

    private void Start()
    {
        _renderer = GetComponent<Renderer>();
        if (_renderer == null)
        {
            Debug.LogWarning("Renderer not found on this GameObject.");
            return;
        }
        // Disable the renderer at the start
        _renderer.enabled = false;
    }

}
