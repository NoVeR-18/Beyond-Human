using UnityEngine;

public class StairZone : MonoBehaviour
{
    public float zStart = 0f;
    public float zEnd = 1f;
    public Vector2 stairDirection = new Vector2(1, 1); // ↗ по умолчанию

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            var controller = other.GetComponent<PlayerController>();
            if (controller != null)
            {
                controller.SetOnStairs(true, zStart, zEnd, stairDirection);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            var controller = other.GetComponent<PlayerController>();
            if (controller != null)
            {
                controller.SetOnStairs(false, 0f, 0f, Vector2.zero);
            }
        }
    }
}
