using Assets.Scripts.NPC;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public float interactDistance = 2f;
    public Vector2 interactionDirection = Vector2.right; // направление взаимодействия, например: вправо

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            TryInteract();
        }
    }

    void TryInteract()
    {
        Vector2 origin = transform.position;
        Vector2 direction = interactionDirection.normalized;

        //RaycastHit2D hit = Physics2D.Raycast(origin, direction, interactDistance, LayerMask.GetMask("Interacteble"));
        Collider2D hit = Physics2D.OverlapCircle(origin, interactDistance, LayerMask.GetMask("Interacteble", "NPC"));

        if (hit != null)
        {
            if (hit.TryGetComponent<IInteractable>(out var interactable))
            {
                if (interactable != null)
                {
                    PlayerKeysAndSkills player = GetComponent<PlayerKeysAndSkills>(); // Предполагаем, что у игрока есть свой скрипт
                    interactable.Interact(player);
                }
            }
            else if (hit.TryGetComponent<NPCController>(out var npc))
            {
                if (npc != null && npc.StateMachine.CurrentState is IInteractableState interactableState)
                    interactableState.Interact(GetComponent<PlayerController>());
            }
        }
    }

    // Визуализация луча в редакторе
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Vector3 dir = (Vector3)interactionDirection.normalized * interactDistance;
        Gizmos.DrawSphere(transform.position, interactDistance);
        Gizmos.DrawLine(transform.position, transform.position + dir);
    }
}
