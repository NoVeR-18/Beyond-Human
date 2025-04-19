using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public float interactDistance = 2f;
    public Vector2 interactionDirection = Vector2.right; // ����������� ��������������, ��������: ������

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

        RaycastHit2D hit = Physics2D.Raycast(origin, direction, interactDistance, LayerMask.NameToLayer("Interacteble"));

        if (hit.collider != null)
        {
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();
            if (interactable != null)
            {
                PlayerKeysAndSkills player = GetComponent<PlayerKeysAndSkills>(); // ������������, ��� � ������ ���� ���� ������
                interactable.Interact(player);
            }
        }
    }

    // ������������ ���� � ���������
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Vector3 dir = (Vector3)interactionDirection.normalized * interactDistance;
        Gizmos.DrawLine(transform.position, transform.position + dir);
    }
}
