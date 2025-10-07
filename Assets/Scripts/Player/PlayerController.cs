using NPCEnums;
using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

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

    [Header("Swimming Settings")]
    [SerializeField] private Tilemap waterTilemap;
    [SerializeField] private Sprite boatSprite;
    [SerializeField] private Sprite normalSprite;
    [SerializeField] private float swimSpeed = 2f;
    private bool isSwimming = false;

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
    private Vector2 stairDirection = Vector2.zero;
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
        spriteRenderer = GetComponent<SpriteRenderer>();

        UpdateDirection(movement);

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

    private void Start()
    {
        if (waterTilemap == null)
            waterTilemap = FindAnyObjectByType<GridHierarchy>().background;
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

    public void LoadFromData(PlayerSaveData data)
    {
        transform.position = data.position;
        transform.rotation = data.rotation;
        battleParticipantData.stats.CurrentHP = data.health;
    }

    void Update()
    {
        HandleInput();
        HandleZPosition();
        if (isSwimming)
            CheckForShoreExit();
    }

    private void HandleInput()
    {
        float inputX = Input.GetAxisRaw("Horizontal");
        float inputY = Input.GetAxisRaw("Vertical");

        if (isSwimming)
        {
            // In water fixed swim speed and free movement
            currentSpeed = swimSpeed;
            movement = new Vector2(inputX, inputY);

            // local check to prevent leaving water
            Vector3Int nextCell = waterTilemap.WorldToCell(transform.position + (Vector3)(movement.normalized * 0.5f));
            if (!waterTilemap.HasTile(nextCell))
                movement = Vector2.zero;

            return;
        }

        // На суше обычное управление
        if (Input.GetKey(KeyCode.LeftShift))
            currentState = MovementState.Run;
        else if (Input.GetKey(KeyCode.LeftControl))
            currentState = MovementState.Crawl;
        else
            currentState = MovementState.Walk;

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
                movement = stairDirection.normalized;
            else if (inputX < -0.01f)
                movement = -stairDirection.normalized;
            else
                movement = new Vector2(0, inputY * stairDirection.magnitude);
        }
        else
        {
            movement = new Vector2(inputX, inputY);
        }
        HandleAnimation();
    }

    private void HandleAnimation()
    {
        animator.SetFloat("Speed", movement.sqrMagnitude);

        if (!isSwimming && movement.sqrMagnitude > 0.01f)
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
            newDir = move.y > 0 ? Direction.Back : Direction.Front;
        }
        else
        {
            newDir = Direction.Side;
        }

        if (newDir != currentDirection)
        {
            currentDirection = newDir;
            playerEquipmentManager?.SetDirection(currentDirection);
        }
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + movement.normalized * currentSpeed * Time.fixedDeltaTime);

        if (!isSwimming && movement.sqrMagnitude > 0.1f)
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

    // === STAIRS ===
    public void SetOnStairs(bool value, float zStart, float zEnd, Vector2 stairDirection)
    {
        onStairs = value;
        this.zStart = zStart;
        this.zEnd = zEnd;
        this.stairDirection = stairDirection;
    }

    // === SWIMMING LOGIC ===
    public void EnterBoatMode(Vector3 worldPos)
    {
        transform.position = worldPos;
        isSwimming = true;
        spriteRenderer.sprite = boatSprite;
        animator.enabled = false; // лодка статичная, отключаем анимации
    }

    private void CheckForShoreExit()
    {
        Vector3Int currentCell = waterTilemap.WorldToCell(transform.position);
        Vector3Int[] offsets =
        {
            new Vector3Int(1,0,0), new Vector3Int(-1,0,0),
            new Vector3Int(0,1,0), new Vector3Int(0,-1,0)
        };

        foreach (var offset in offsets)
        {
            var checkCell = currentCell + offset;
            if (!waterTilemap.HasTile(checkCell))
            {
                ExitBoatMode(waterTilemap.CellToWorld(checkCell) + new Vector3(0.5f, 0.5f, 0));
                break;
            }
        }
    }

    private void ExitBoatMode(Vector3 shorePos)
    {
        transform.position = shorePos;
        isSwimming = false;
        spriteRenderer.sprite = normalSprite;
        animator.enabled = true;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (string.IsNullOrEmpty(battleParticipantData.nameID))
        {
            battleParticipantData.nameID = GenerateId();
            EditorUtility.SetDirty(this);
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
