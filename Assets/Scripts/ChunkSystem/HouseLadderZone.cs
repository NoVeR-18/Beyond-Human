using Assets.Scripts.NPC;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class HouseLadderZone : MonoBehaviour
{
    public FloorManager floorManager;
    public int TargetFloor = 1;
    public int IndextFloor = 1;
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
        if (other.CompareTag("Player"))
        {
            if (playerWasInside)
            {
                playerWasInside = false;
                return;
            }

            floorManager?.ChangeFloor(TargetFloor);
            readyToTrigger = false;
        }
        else if (other.TryGetComponent<NPCController>(out var npc))
        {
            // NPC поднимается/спускается
            if (npc.CurrentFloor == TargetFloor)
            {
                npc.CurrentFloor = IndextFloor;
            }
            else
                npc.CurrentFloor = TargetFloor;
            npc.CurrentHouse = floorManager.house;

            floorManager.UpdateNPCVisibility(npc);
        }


    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        StopCoroutine(CheckPlayerInsideBeforeEnabling());
        playerWasInside = false;
        readyToTrigger = true;
    }
}
