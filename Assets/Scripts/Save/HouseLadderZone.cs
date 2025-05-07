using UnityEngine;


[RequireComponent(typeof(BoxCollider2D))]

public class HouseLadderZone : MonoBehaviour
{
    public FloorManager floorManager;
    [Range(-1, 1)]
    public int direction = 1;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (floorManager == null)
        {
            Debug.LogWarning("HouseLadderZone: FloorManager не назначен.");
            return;
        }

        floorManager.ChangeFloor(direction);
    }
}