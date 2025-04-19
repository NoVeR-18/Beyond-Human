using UnityEngine;

public class Chest : MonoBehaviour, IInteractable
{
    public bool isLocked = false;
    public int requiredLockpickingLevel = 0;
    public bool isOpened = false;
    private Animator animator;
    private void Start()
    {
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("Animator component is missing on the chest.");
        }
    }
    public void Interact(PlayerKeysAndSkills player)
    {
        if (isOpened)
        {
            CloseChest();
            Debug.Log("Chest is opened.");
            return;
        }

        if (isLocked && player.lockpickingLevel < requiredLockpickingLevel)
        {
            Debug.Log("The hack level is too low.");
            return;
        }

        OpenChest();
    }

    void OpenChest()
    {
        isOpened = true;
        animator.SetTrigger("Open");
        Debug.Log("Chest opened! Rewards received.");
        // Here you can give a reward to the player
    }
    void CloseChest()
    {
        isOpened = false;
        animator.SetTrigger("Close");
        Debug.Log("Chest closed!");
    }
}