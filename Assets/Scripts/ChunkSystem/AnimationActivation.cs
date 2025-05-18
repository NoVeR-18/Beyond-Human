using UnityEngine;

public class AnimationActivation : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<IDynamicObject>(out var dynamic))
            dynamic.SetAnimationActive(true);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent<IDynamicObject>(out var dynamic))
            dynamic.SetAnimationActive(false);
    }
}