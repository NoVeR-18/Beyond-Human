using GameUtils.Utils;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NPCRender))]
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class EnemyAI : MonoBehaviour
{
    public enum State
    {
        Idle,
        Roaming,
        Chasing,
        GoingToLastSeenPosition
    }

    [SerializeField] private State startingState = State.Roaming;

    [Header("Moving settings")]
    [SerializeField] private float idleDuration = 2f;
    [SerializeField] private float roamTimerMax = 2f;
    [SerializeField] private float roamingDistanceMax = 7f;
    [SerializeField] private float roamingDistanceMin = 3f;

    [Header("Field of view")]
    [SerializeField] private float viewDistance = 6f;
    [SerializeField] private float viewAngle = 90f;
    [SerializeField] private LayerMask obstacleMask;
    [SerializeField] private LayerMask playerMask;

    [Header("Chase settings")]
    [SerializeField] private float chaseMemoryDuration = 3f;
    [SerializeField] private float catchDistance = 1.5f;

    private Transform player;
    private NavMeshAgent navMeshAgent;
    private Animator animator;
    [SerializeField] private State state;
    private float stateTimer;
    private Vector3 roamPosition;
    private Vector3 startingPosition;
    private Vector3 lastSeenPlayerPosition;
    private float chaseMemoryTimer;

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        navMeshAgent.updateRotation = false;
        navMeshAgent.updateUpAxis = false;

        state = startingState;
        stateTimer = roamTimerMax;
    }

    public void SetTarget(Transform target)
    {
        player = target;
    }

    private void Update()
    {

        switch (state)
        {
            case State.Roaming:
                if (CanSeePlayer())
                {
                    state = State.Chasing;
                    chaseMemoryTimer = chaseMemoryDuration;
                }
                else if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
                {
                    state = State.Idle;
                    stateTimer = idleDuration;
                    navMeshAgent.ResetPath();
                }
                break;

            case State.Idle:
                if (CanSeePlayer())
                {
                    state = State.Chasing;
                    chaseMemoryTimer = chaseMemoryDuration;
                }
                else
                {
                    stateTimer -= Time.deltaTime;
                    if (stateTimer <= 0f)
                    {
                        state = State.Roaming;
                        StartRoaming();
                    }
                }
                break;

            case State.Chasing:
                if (CanSeePlayer())
                {
                    chaseMemoryTimer = chaseMemoryDuration;
                    lastSeenPlayerPosition = player.position;

                    navMeshAgent.SetDestination(player.position);
                    ChangeFacingDirection(transform.position, player.position);

                    if (Vector3.Distance(transform.position, player.position) <= catchDistance)
                    {
                        navMeshAgent.ResetPath();
                        Debug.Log("Caught");
                    }
                }
                else
                {
                    chaseMemoryTimer -= Time.deltaTime;
                    if (chaseMemoryTimer <= 0f)
                    {
                        state = State.GoingToLastSeenPosition;
                        navMeshAgent.SetDestination(lastSeenPlayerPosition);
                    }
                }
                break;

            case State.GoingToLastSeenPosition:
                if (CanSeePlayer())
                {
                    state = State.Chasing;
                    chaseMemoryTimer = chaseMemoryDuration;
                }
                else if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
                {
                    state = State.Roaming;
                    StartRoaming();
                }
                break;
        }

        animator.SetFloat("Speed", navMeshAgent.velocity.magnitude);
    }

    private void StartRoaming()
    {
        startingPosition = transform.position;
        roamPosition = GetRoamingPosition();
        ChangeFacingDirection(startingPosition, roamPosition);
        navMeshAgent.SetDestination(roamPosition);
    }

    private Vector3 GetRoamingPosition()
    {
        return startingPosition + Utils.GetRandomDir() * Random.Range(roamingDistanceMin, roamingDistanceMax);
    }

    private void ChangeFacingDirection(Vector3 sourcePosition, Vector3 targetPosition)
    {
        if (sourcePosition.x > targetPosition.x)
            transform.rotation = Quaternion.Euler(0, -180, 0);
        else
            transform.rotation = Quaternion.Euler(0, 0, 0);
    }

    private bool CanSeePlayer()
    {
        // Search for all colliders within view radius, filtered by player layer
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, viewDistance, playerMask);

        foreach (var hit in hits)
        {
            Transform potentialTarget = hit.transform;
            Vector3 directionToTarget = (potentialTarget.position - transform.position).normalized;
            float distanceToTarget = Vector3.Distance(transform.position, potentialTarget.position);

            float angle = Vector3.Angle(transform.right, directionToTarget);
            if (angle < viewAngle / 2f)
            {
                // Check if there are any obstacles between the enemy and the player
                RaycastHit2D rayHit = Physics2D.Raycast(transform.position, directionToTarget, distanceToTarget, obstacleMask);
                if (!rayHit)
                {
                    // Unobstructed player detected
                    player = potentialTarget;
                    return true;
                }
            }
        }

        return false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, viewDistance);

        Vector3 leftDir = Quaternion.Euler(0, 0, -viewAngle / 2) * transform.right;
        Vector3 rightDir = Quaternion.Euler(0, 0, viewAngle / 2) * transform.right;

        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + leftDir * viewDistance);
        Gizmos.DrawLine(transform.position, transform.position + rightDir * viewDistance);
    }
}
