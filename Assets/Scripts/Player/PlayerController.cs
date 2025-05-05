using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;

    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private Vector2 movement;

    // === Лестница ===
    private bool onStairs = false;
    private float zStart = 0f;
    private float zEnd = 1f;
    private Vector2 stairDirection = Vector2.zero; // ↗ направление лестницы

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        float inputX = Input.GetAxisRaw("Horizontal");

        if (onStairs && stairDirection != Vector2.zero)
        {
            // Управляем движением по лестнице (влево/вправо → вниз/вверх)
            if (inputX > 0.01f)
            {
                movement = stairDirection.normalized;     // вверх по лестнице
            }
            else if (inputX < -0.01f)
            {
                movement = -stairDirection.normalized;    // вниз по лестнице
            }
            else
            {
                movement = Vector2.zero;
                movement.y = Input.GetAxisRaw("Vertical") * stairDirection.magnitude;

            }
        }
        else
        {
            // Обычное движение
            movement.x = inputX;
            movement.y = Input.GetAxisRaw("Vertical");
        }

        animator.SetFloat("Speed", movement.sqrMagnitude);
        Flip();

        // Z-позиция и сортировка на лестнице
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
    }

    void Flip()
    {
        if (movement.x > 0.01f)
            spriteRenderer.flipX = false;
        else if (movement.x < -0.01f)
            spriteRenderer.flipX = true;
    }

    // === Вход на лестницу ===
    public void SetOnStairs(bool value, float zStart, float zEnd, Vector2 stairDirection)
    {
        onStairs = value;
        this.zStart = zStart;
        this.zEnd = zEnd;
        this.stairDirection = stairDirection;
    }
}
