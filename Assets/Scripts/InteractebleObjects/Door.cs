using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Door : InteractableObject, IInteractable
{
    public bool requiresKey = false;
    public string requiredKeyID;

    private BoxCollider2D doorCollider;
    private Animator animator;
    [SerializeField]
    private bool isOpen = false;

    private void Start()
    {
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("Missing Animator on door.");
        }

        doorCollider = GetComponent<BoxCollider2D>();
        ApplyVisualState();
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
            Debug.Log("You donТt have the required key.");
            return;
        }

        OpenDoor();
    }

    private void OpenDoor()
    {
        isOpen = true;
        ApplyVisualState();
        Debug.Log("Door is open.");
    }

    private void CloseDoor()
    {
        isOpen = false;
        ApplyVisualState();
        Debug.Log("Door is closed.");
    }

    private void ApplyVisualState()
    {
        if (animator != null)
        {
            animator.SetTrigger(isOpen ? "Open" : "Close");

        }

        if (doorCollider != null)
        {
            doorCollider.isTrigger = isOpen;
        }
    }

    // --- —охранение и загрузка состо€ни€ двери ---
    public override InteractableSaveData GetSaveData()
    {
        return new InteractableSaveData
        {
            id = GetID(),
            isOpened = this.isOpen, // если используетс€
            isDestroyed = false, // если применимо

            position = transform.position,
            rotation = transform.rotation,
        };
    }

    public override void LoadFromData(InteractableSaveData data)
    {
        this.isOpen = data.isOpened;
        ApplyVisualState();
    }

    public override void Destroy()
    {
    }
}
