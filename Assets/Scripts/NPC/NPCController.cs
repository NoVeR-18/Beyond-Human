using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class NPCController : MonoBehaviour
{
    public bool isAggressive;
    public float viewDistance = 6f;
    public float viewAngle = 90f;
    public LayerMask obstacleMask, playerMask;

    public NPCSchedule schedule;

    [HideInInspector] public NavMeshAgent agent;
    [HideInInspector] public Transform target;
    [HideInInspector] public NPCStateMachine stateMachine;
    public string StateName;


    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        stateMachine = new NPCStateMachine(this);
    }

    private void Start()
    {
        stateMachine.ChangeState(new IdleState());
    }

    private void Update()
    {
        stateMachine.CurrentState?.Update();
    }

    public bool CanSeePlayer(out Transform player)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, viewDistance, playerMask);

        foreach (var hit in hits)
        {
            Vector3 dir = (hit.transform.position - transform.position).normalized;
            float angle = Vector3.Angle(transform.right, dir);
            float dist = Vector3.Distance(transform.position, hit.transform.position);

            if (angle < viewAngle / 2f && !Physics2D.Raycast(transform.position, dir, dist, obstacleMask))
            {
                player = hit.transform;
                return true;
            }
        }

        player = null;
        return false;
    }
}
