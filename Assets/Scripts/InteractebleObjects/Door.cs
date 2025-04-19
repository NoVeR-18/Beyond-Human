using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Door : MonoBehaviour, IInteractable
{
    public bool requiresKey = false;
    public string requiredKeyID;

    private BoxCollider2D doorCollider;

    private Animator animator;
    private bool isOpen = false;

    private void Start()
    {
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("missing Animator on door.");
        }

        doorCollider = GetComponent<BoxCollider2D>();
    }

    public void Interact(PlayerKeysAndSkills player)
    {
        if (isOpen)
        {
            CloseDoor();
            return;
        }

        if (requiresKey && !player.HasKey(requiredKeyID))
        {
            Debug.Log("You don`t have key.");
            return;
        }

        OpenDoor();
    }

    void OpenDoor()
    {
        isOpen = true;
        animator.SetTrigger("Open");
        doorCollider.isTrigger = true;
        Debug.Log("Door is open.");
    }
    void CloseDoor()
    {
        isOpen = false;
        animator.SetTrigger("Close");
        doorCollider.isTrigger = false;
        Debug.Log("Door is close.");
    }

}
