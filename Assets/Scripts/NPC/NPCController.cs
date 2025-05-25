using Assets.Scripts.NPC.Dialogue;
using Assets.Scripts.NPC.States;
using NPCEnums;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Assets.Scripts.NPC
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class NPCController : MonoBehaviour
    {
        [Header("Global settings")]
        public bool isAggressive;
        public float viewDistance = 6f;
        public float viewAngle = 90f;
        public LayerMask obstacleMask, playerMask;

        [Header("NPC Schedule")]
        public List<ScheduleEntry> schedule = new();

        [HideInInspector] public NavMeshAgent Agent;
        [HideInInspector] public Animator Animator;
        [HideInInspector] public Transform target;
        [HideInInspector] public NPCStateMachine StateMachine;

        public string StateName;

        private ScheduleEntry currentEntry;

        public NPCDialogueSet dialogueSet;

        private InteractionEmitter emitter;
        public void Speak(DialogueContext context)
        {
            var lines = dialogueSet?.GetRandomDialogue(context);
            if (lines == null || lines.Count == 0) return;

            StartCoroutine(RunDialogue(lines));
        }

        private IEnumerator RunDialogue(List<DialogueLine> lines)
        {
            foreach (var line in lines)
            {
                Debug.Log($"{gameObject.name} says: {line.text}");
                UIManager.Instance.dialogueWindow.ShowDialogue(line.text);

                //  Here can integrate in UI system
                yield return new WaitForSeconds(line.delayAfter);
            }
        }


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
            if (emitter == null)
            {
                var prefab = Resources.Load<InteractionEmitter>("Prefabs/InteractionEmitter");
                emitter = Instantiate(prefab, transform.position, Quaternion.identity, transform);
            }
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
            ScheduleEntry bestEntry = null;

            foreach (var entry in schedule)
            {
                if (entry.day != gameTime.Day) continue;

                int entryTime = entry.hour * 60 + entry.minute;
                int currentTime = gameTime.Hour * 60 + gameTime.Minute;

                if (entryTime <= currentTime)
                {
                    if (bestEntry == null)
                    {
                        bestEntry = entry;
                    }
                    else
                    {
                        int bestTime = bestEntry.hour * 60 + bestEntry.minute;
                        if (entryTime > bestTime)
                            bestEntry = entry;
                    }
                }
            }

            return bestEntry;
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

        private Coroutine dialogueRoutine;

        public void StartContextDialogue(DialogueContext context)
        {
            if (dialogueRoutine != null)
                StopCoroutine(dialogueRoutine);

            dialogueRoutine = StartCoroutine(ContextDialogueRoutine(context));
        }

        public void StopContextDialogue()
        {
            if (dialogueRoutine != null)
                StopCoroutine(dialogueRoutine);
            dialogueRoutine = null;
        }

        private IEnumerator ContextDialogueRoutine(DialogueContext context)
        {
            while (true)
            {
                var lines = dialogueSet?.GetRandomDialogue(context);
                if (lines != null && lines.Count > 0)
                {
                    var line = lines[UnityEngine.Random.Range(0, lines.Count)];
                    ShowFloatingText(line.text);
                }

                yield return new WaitForSeconds(UnityEngine.Random.Range(10f, 25f)); // случайный интервал
            }
        }

        private void ShowFloatingText(string text)
        {
            UIFloatingText.Create(transform.position + Vector3.up * 1.5f, text);
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



}