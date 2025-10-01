using NPCEnums;
using System;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour, IFactionMember
{
    public InteractionEmitter emitter;
    [Header("Movement Settings")]
    public float walkSpeed = 3f;
    public float runSpeed = 6f;
    public float crawlSpeed = 1.5f;
    private float currentSpeed;

    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    public BattleParticipantData battleParticipantData;
    private Vector2 movement;
    public EquipmentManager playerEquipmentManager;

    // === Stairs ===
    private bool onStairs = false;
    private float zStart = 0f;
    private float zEnd = 1f;
    private Vector2 stairDirection = Vector2.zero; // ↗ Direction of the stairs
    private Direction currentDirection = Direction.Front;

    private enum MovementState
    {
        Walk,
        Run,
        Crawl
    }

    private MovementState currentState = MovementState.Walk;

    private FactionData factionData;
    public FactionData FactionData => factionData;
    [SerializeField] private FactionType factionType;

    public FactionType FactionType => factionType;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        UpdateDirection(movement);
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (emitter == null)
        {
            var prefab = Resources.Load<InteractionEmitter>("Prefabs/InteractionEmitter");
            emitter = Instantiate(prefab, transform.position, Quaternion.identity, transform);
            emitter.tag = "Player";
        }
        factionData = FactionManager.Instance.GetFaction(factionType);
        if (factionData == null)
            Debug.LogWarning($"FactionData not found for {factionType} on {gameObject.name}");
    }
    public PlayerSaveData GetSaveData()
    {
        return new PlayerSaveData
        {
            position = transform.position,
            rotation = transform.rotation,
            health = battleParticipantData.stats.CurrentHP
        };
    }

    // Load player data from save
    public void LoadFromData(PlayerSaveData data)
    {
        transform.position = data.position;
        transform.rotation = data.rotation;
        battleParticipantData.stats.CurrentHP = data.health;
    }

    void Update()
    {
        HandleInput();
        HandleAnimation();
        HandleZPosition();
    }

    private void HandleInput()
    {
        float inputX = Input.GetAxisRaw("Horizontal");
        float inputY = Input.GetAxisRaw("Vertical");

        // переключение состояний (пример — Shift = бег, Ctrl = ползание)
        if (Input.GetKey(KeyCode.LeftShift))
            currentState = MovementState.Run;
        else if (Input.GetKey(KeyCode.LeftControl))
            currentState = MovementState.Crawl;
        else
            currentState = MovementState.Walk;

        // выбор скорости в зависимости от состояния
        switch (currentState)
        {
            case MovementState.Walk:
                currentSpeed = walkSpeed;
                break;
            case MovementState.Run:
                currentSpeed = runSpeed;
                break;
            case MovementState.Crawl:
                currentSpeed = crawlSpeed;
                break;
        }

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
                movement = new Vector2(0, inputY * stairDirection.magnitude);
            }
        }
        else
        {
            movement = new Vector2(inputX, inputY);
        }
    }


    private void HandleAnimation()
    {
        animator.SetFloat("Speed", movement.sqrMagnitude);

        if (movement.sqrMagnitude > 0.01f)
        {
            animator.SetFloat("MoveX", movement.x);
            animator.SetFloat("MoveY", movement.y);
            UpdateDirection(movement);
        }

        Flip();
    }

    private void HandleZPosition()
    {
        if (onStairs)
        {
            float progress = Mathf.InverseLerp(zStart, zEnd, transform.position.y);
            float z = Mathf.Lerp(zStart, zEnd, progress);
            transform.position = new Vector3(transform.position.x, transform.position.y, z);
            spriteRenderer.sortingOrder = Mathf.RoundToInt(z * 10);
        }
    }


    private void UpdateDirection(Vector2 move)
    {
        Direction newDir;

        if (Mathf.Abs(move.y) > Mathf.Abs(move.x))
        {
            if (move.y > 0)
                newDir = Direction.Back;
            else
                newDir = Direction.Front;
        }
        else
        {
            newDir = Direction.Side;
        }

        if (newDir != currentDirection)
        {
            currentDirection = newDir;
            if (playerEquipmentManager != null)
                playerEquipmentManager?.SetDirection(currentDirection);
        }
    }


    void FixedUpdate()
    {
        rb.MovePosition(rb.position + movement.normalized * currentSpeed * Time.fixedDeltaTime);
        if (movement.sqrMagnitude > 0.1f)
        {
            emitter.Activate(InterruptReason.PlayerWalking, currentSpeed / 2);
        }
    }

    void Flip()
    {
        if (Mathf.Abs(movement.x) > 0.01f)
        {
            spriteRenderer.flipX = movement.x < 0f;
        }
    }

    // === Enter on stairs ===
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
        // Auto-generate nameID if it's empty
        if (string.IsNullOrEmpty(battleParticipantData.nameID))
        {
            battleParticipantData.nameID = GenerateId();
            EditorUtility.SetDirty(this); // Mark the object as dirty to save changes
        }
    }
    public bool IsEnemyTo(IFactionMember other)
    {
        if (factionData == null || other.FactionData == null) return false;
        return factionData.IsHostileTowards(other.FactionData);
    }

    private string GenerateId()
    {
        return $"{gameObject.scene.name}_{gameObject.name}_{Guid.NewGuid().ToString().Substring(0, 8)}";
    }
#endif
}
