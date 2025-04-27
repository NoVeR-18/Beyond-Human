using UnityEngine;
public class RenderActivation : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<IDynamicObject>(out var dynamic))
            dynamic.SetRenderActive(true);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent<IDynamicObject>(out var dynamic))
            dynamic.SetRenderActive(false);
    }
}
