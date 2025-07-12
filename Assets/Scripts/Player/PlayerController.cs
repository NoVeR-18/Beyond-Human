using NPCEnums;
using System;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    public InteractionEmitter emitter;
    public float moveSpeed = 5f;

    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    public BattleParticipantData battleParticipantData;
    private Vector2 movement;

    // === Лестница ===
    private bool onStairs = false;
    private float zStart = 0f;
    private float zEnd = 1f;
    private Vector2 stairDirection = Vector2.zero; // ↗ направление лестницы

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (emitter == null)
        {
            var prefab = Resources.Load<InteractionEmitter>("Prefabs/InteractionEmitter");
            emitter = Instantiate(prefab, transform.position, Quaternion.identity, transform);
        }
    }

    void Update()
    {
        float inputX = Input.GetAxisRaw("Horizontal");
        float inputY = Input.GetAxisRaw("Vertical");

        if (onStairs && stairDirection != Vector2.zero)
        {
            if (inputX > 0.01f)
            {
                movement = stairDirection.normalized;
            }
            else if (inputX < -0.01f)
            {
                movement = -stairDirection.normalized;
            }
            else
            {
                movement = Vector2.zero;
                movement.y = Input.GetAxisRaw("Vertical") * stairDirection.magnitude;
            }
        }
        else
        {
            movement = new Vector2(inputX, inputY);
        }

        // Обновление параметров анимации
        animator.SetFloat("Speed", movement.sqrMagnitude);
        if (movement.sqrMagnitude > 0.01f)
        {
            animator.SetFloat("MoveX", movement.x);
            animator.SetFloat("MoveY", movement.y);
        }

        Flip();

        // Обработка Z-позиции на лестнице
        if (onStairs)
        {
            float progress = Mathf.InverseLerp(zStart, zEnd, transform.position.y);
            float z = Mathf.Lerp(zStart, zEnd, progress);
            transform.position = new Vector3(transform.position.x, transform.position.y, z);
            spriteRenderer.sortingOrder = Mathf.RoundToInt(z * 10);
        }
    }



    void FixedUpdate()
    {
        rb.MovePosition(rb.position + movement.normalized * moveSpeed * Time.fixedDeltaTime);
        if (movement.sqrMagnitude > 0.1f)
        {
            emitter.Activate(InterruptReason.PlayerWalking);
        }
    }

    void Flip()
    {
        if (Mathf.Abs(movement.x) > 0.01f)
        {
            spriteRenderer.flipX = movement.x < 0f;
        }
    }

    // === Вход на лестницу ===
    public void SetOnStairs(bool value, float zStart, float zEnd, Vector2 stairDirection)
    {
        onStairs = value;
        this.zStart = zStart;
        this.zEnd = zEnd;
        this.stairDirection = stairDirection;
    }
#if UNITY_EDITOR
    private void OnValidate()
    {
        // Автогенерация при добавлении компонента
        if (string.IsNullOrEmpty(battleParticipantData.nameID))
        {
            battleParticipantData.nameID = GenerateId();
            EditorUtility.SetDirty(this); // помечаем объект как изменённый
        }
    }

    private string GenerateId()
    {
        return $"{gameObject.scene.name}_{gameObject.name}_{Guid.NewGuid().ToString().Substring(0, 8)}";
    }
#endif
}
