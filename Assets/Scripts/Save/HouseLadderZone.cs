using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class HouseLadderZone : MonoBehaviour
{
    public FloorManager floorManager;
    public int direction = 1;

    private bool readyToTrigger = false;
    private bool playerWasInside = false;
    private Collider2D triggerCollider;

    private void OnEnable()
    {
        triggerCollider = GetComponent<Collider2D>();
        StartCoroutine(CheckPlayerInsideBeforeEnabling());
    }

    private IEnumerator CheckPlayerInsideBeforeEnabling()
    {
        yield return new WaitForSeconds(0.1f);

        Collider2D[] results = new Collider2D[5];
        ContactFilter2D filter = new ContactFilter2D();
        filter.NoFilter();

        int count = triggerCollider.Overlap(filter, results);
        playerWasInside = false;

        for (int i = 0; i < count; i++)
        {
            if (results[i] != null && results[i].CompareTag("Player"))
            {
                playerWasInside = true;
                break;
            }
        }

        readyToTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!readyToTrigger) return;
        if (!other.CompareTag("Player")) return;

        if (playerWasInside)
        {
            playerWasInside = false;
            return;
        }

        floorManager?.ChangeFloor(direction);
        readyToTrigger = false;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        StopCoroutine(CheckPlayerInsideBeforeEnabling());
        playerWasInside = false;
        readyToTrigger = true;
    }
}
