using NPCEnums;
using System;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour, IFactionMember
{
    public InteractionEmitter emitter;
    public float moveSpeed = 5f;

    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    public BattleParticipantData battleParticipantData;
    private Vector2 movement;
    public EquipmentManager playerEquipmentManager;

    // === Лестница ===
    private bool onStairs = false;
    private float zStart = 0f;
    private float zEnd = 1f;
    private Vector2 stairDirection = Vector2.zero; // ↗ направление лестницы
    private Direction currentDirection = Direction.Front;

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

    // Загрузка данных
    public void LoadFromData(PlayerSaveData data)
    {
        transform.position = data.position;
        transform.rotation = data.rotation;
        battleParticipantData.stats.CurrentHP = data.health;
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


            UpdateDirection(movement);
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
            playerEquipmentManager?.SetDirection(currentDirection);
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
