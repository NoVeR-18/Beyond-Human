using UnityEngine;

public class LogicActivation : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<IDynamicObject>(out var dynamic))
            dynamic.SetLogicActive(true);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent<IDynamicObject>(out var dynamic))
            dynamic.SetLogicActive(false);
    }
}