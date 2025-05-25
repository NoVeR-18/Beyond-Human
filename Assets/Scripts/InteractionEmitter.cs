using Assets.Scripts.NPC;
using NPCEnums;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class InteractionEmitter : MonoBehaviour
{
    public CircleCollider2D triggerCollider;
    public InterruptReason currentReason;
    public SpriteRenderer spriteRenderer;

    private Coroutine activeCoroutine;
    private float remainingTime;

    private void Awake()
    {
        triggerCollider.enabled = false;
        spriteRenderer.size = Vector2.zero;
    }

    public void Activate(InterruptReason reason, float radius = 1f, float duration = 0.1f)
    {
        currentReason = reason;
        triggerCollider.radius = radius;
        spriteRenderer.size = new Vector2(radius * 2f, radius * 2f);
        triggerCollider.enabled = true;

        remainingTime = duration;

        if (activeCoroutine == null)
        {
            activeCoroutine = StartCoroutine(InteractionCoroutine());
        }
    }

    private IEnumerator InteractionCoroutine()
    {
        while (remainingTime > 0f)
        {
            remainingTime -= Time.deltaTime;
            yield return null;
        }

        triggerCollider.enabled = false;
        triggerCollider.radius = 0f;
        spriteRenderer.size = Vector2.zero;
        activeCoroutine = null;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        var interruptible = other.GetComponent<IInterruptible>();
        if (interruptible != null)
        {
            interruptible.Interrupt(transform.root.GetComponent<NPCController>(), currentReason);
        }
    }
}

