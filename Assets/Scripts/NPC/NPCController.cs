using NPCEnums;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class NPCController : MonoBehaviour
{
    [Header("Параметры поведения")]
    public bool isAggressive;
    public float viewDistance = 6f;
    public float viewAngle = 90f;
    public LayerMask obstacleMask, playerMask;

    [Header("Расписание NPC")]
    public List<ScheduleEntry> schedule = new();

    [HideInInspector] public NavMeshAgent Agent;
    [HideInInspector] public Animator Animator;
    [HideInInspector] public Transform target;
    [HideInInspector] public NPCStateMachine StateMachine;

    public string StateName;

    private ScheduleEntry currentEntry;

    private void Awake()
    {
        Agent = GetComponent<NavMeshAgent>();
        Animator = GetComponent<Animator>();
        StateMachine = new NPCStateMachine(this);
    }

    private void Start()
    {
        StateMachine.ChangeState(new IdleState(this));
        TimeManager.Instance.OnTimeChanged += OnTimeChanged;
    }

    private void OnDestroy()
    {
        TimeManager.Instance.OnTimeChanged -= OnTimeChanged;
    }

    private void Update()
    {
        StateMachine.CurrentState?.Update();
    }

    private void OnTimeChanged(GameTime time)
    {
        ScheduleEntry next = GetScheduleEntry(time);
        if (next != null && next != currentEntry)
        {
            currentEntry = next;
            SwitchActivity(next);
        }
    }

    private ScheduleEntry GetScheduleEntry(GameTime gameTime)
    {
        ScheduleEntry closest = null;
        foreach (var entry in schedule)
        {
            if (entry.day == gameTime.Day)
                if (entry.hour == gameTime.Hour && entry.minute <= gameTime.Minute)
                {
                    if (closest == null || entry.minute > closest.minute)
                        closest = entry;
                }
        }
        return closest;
    }
    private void SwitchActivity(ScheduleEntry entry)
    {
        switch (entry.activity)
        {
            case NPCActivityType.Idle:
                StateMachine.ChangeState(new IdleState(this));
                break;

            case NPCActivityType.Guard:
                StateMachine.ChangeState(new RoamState(this));
                break;

            case NPCActivityType.Work:
                StateMachine.ChangeState(new GoToLocationState(this, entry.destination.position, () =>
                {
                    StateMachine.ChangeState(new WorkState(this)); // Здесь ты можешь выполнять работу (анимация, диалог и т.д.)
                }));
                break;

            case NPCActivityType.Sleep:
                StateMachine.ChangeState(new GoToLocationState(this, entry.destination.position, () =>
                {
                    StateMachine.ChangeState(new SleepState(this));
                }));
                break;

            case NPCActivityType.Trade:
                StateMachine.ChangeState(new GoToLocationState(this, entry.destination.position, () =>
                {
                    StateMachine.ChangeState(new TradeState(this));
                }));
                break;

            default:
                StateMachine.ChangeState(new IdleState(this));
                break;
        }
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
[System.Serializable]
public class ScheduleEntry
{
    [Range(0, 23)] public int hour;
    [Range(0, 59)] public int minute;
    public DayOfWeek day;
    public NPCActivityType activity;
    public Transform destination;
}


