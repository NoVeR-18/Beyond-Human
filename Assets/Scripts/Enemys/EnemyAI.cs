using GameUtils.Utils;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class EnemyAI : MonoBehaviour
{
    [SerializeField] private State startingState = State.Roaming;
    [SerializeField] private float roamingDistanceMax = 7f;
    [SerializeField] private float roamingDistanceMin = 3f;
    [SerializeField] private float idleDuration = 2f;

    private NavMeshAgent navMeshAgent;
    private Animator animator;

    private State state;
    private float stateTimer;
    private Vector3 roamPosition;
    private Vector3 startingPosition;

    private enum State
    {
        Idle,
        Roaming
    }

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        navMeshAgent.updateRotation = false;
        navMeshAgent.updateUpAxis = false;

        state = startingState;
        stateTimer = idleDuration;
    }

    private void Update()
    {
        switch (state)
        {
            case State.Roaming:
                if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
                {
                    state = State.Idle;
                    stateTimer = idleDuration;
                    navMeshAgent.ResetPath(); // остановка движения
                }
                break;

            case State.Idle:
                stateTimer -= Time.deltaTime;
                if (stateTimer <= 0f)
                {
                    StartRoaming();
                }
                break;
        }

        // Управление анимацией
        animator.SetFloat("Speed", navMeshAgent.velocity.magnitude);
    }

    private void StartRoaming()
    {
        startingPosition = transform.position;
        roamPosition = GetRoamingPosition();
        ChangeFacingDirection(startingPosition, roamPosition);
        navMeshAgent.SetDestination(roamPosition);

        state = State.Roaming;
    }

    private Vector3 GetRoamingPosition()
    {
        return startingPosition + Utils.GetRandomDir() * Random.Range(roamingDistanceMin, roamingDistanceMax);
    }

    private void ChangeFacingDirection(Vector3 sourcePosition, Vector3 targetPosition)
    {
        if (sourcePosition.x > targetPosition.x)
        {
            transform.rotation = Quaternion.Euler(0, -180, 0);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }
}
