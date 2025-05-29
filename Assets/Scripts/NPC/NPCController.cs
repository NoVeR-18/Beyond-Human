using Assets.Scripts.NPC.Dialogue;
using Assets.Scripts.NPC.States;
using Assets.Scripts.NPC.States.Assets.Scripts.NPC.States;
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
        private Dictionary<NPCActivityType, Func<ScheduleEntry, INPCState>> _activityStateFactory;

        [HideInInspector] public HouseData CurrentHouse;
        [HideInInspector] public int CurrentFloor;


        private void InitializeStateFactory()
        {
            _activityStateFactory = new Dictionary<NPCActivityType, Func<ScheduleEntry, INPCState>>
            {
                { NPCActivityType.Idle, _ => new IdleState(this) },
                { NPCActivityType.Guard, entry => CreateGoTo(entry, () => new GuardState(this, entry.destination.transform)) },
                { NPCActivityType.Patrol, entry => CreateGoTo(entry, () => new GuardState(this))},
                { NPCActivityType.Work, entry => CreateGoTo(entry, () => new WorkState(this)) },
                { NPCActivityType.Sleep, entry => CreateGoTo(entry, () => new SleepState(this)) },
                { NPCActivityType.Trade, entry => CreateGoTo(entry, () => new TradeState(this)) },
                { NPCActivityType.Wander, _ => new RoamState(this) },
                { NPCActivityType.Hide, entry => CreateGoTo(entry, () => new HiddenState(this)) },
                { NPCActivityType.Chill, entry => CreateGoTo(entry, () => new ChillState(this, GetDestination(entry))) },
                { NPCActivityType.Hunt, _ => new HuntState(this) },
            };
        }

        [HideInInspector] public InteractionEmitter emitter;
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
        [HideInInspector] public string npcId;
        [HideInInspector] public NPCActivityType CurrentActivity { get; private set; } = NPCActivityType.Idle;

        public NPCSaveData GetSaveData()
        {
            return new NPCSaveData
            {
                npcId = npcId,
                position = transform.position,
                CurrentHouse = CurrentHouse,
                CurrentFloor = CurrentFloor,
                currentActivity = CurrentActivity
            };
        }

        public void LoadFromData(NPCSaveData data)
        {
            transform.position = data.position;
            CurrentHouse = data.CurrentHouse;
            CurrentFloor = data.CurrentFloor;
            SwitchActivity(new ScheduleEntry
            {
                activity = data.currentActivity,
                destination = null // можно доработать для восстановления destination
            });
        }

        private void Awake()
        {
            Agent = GetComponent<NavMeshAgent>();
            Animator = GetComponent<Animator>();
            StateMachine = new NPCStateMachine(this);
            npcId = gameObject.GetInstanceID().ToString();
            SaveSystem.Instance.RegisterNPC(this);
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
            if (_activityStateFactory == null)
                InitializeStateFactory();

            if (_activityStateFactory.TryGetValue(entry.activity, out var factory))
            {
                StateMachine.ChangeState(factory(entry));
                CurrentActivity = entry.activity;
            }
            else
            {
                Debug.LogWarning($"{name}: Unknown activity type {entry.activity}, switching to Idle.");
                StateMachine.ChangeState(new IdleState(this));
            }
        }
        private INPCState CreateGoTo(ScheduleEntry entry, Func<INPCState> nextState)
        {
            if (entry.destination.transform.position != null)
            {
                return new GoToLocationState(this, entry.destination, () =>
                {
                    StateMachine.ChangeState(nextState());
                });
            }
            else
            {
                Debug.LogWarning($"{name}: Destination missing for {entry.activity}, skipping movement.");
                return nextState();
            }
        }

        private Vector3 GetDestination(ScheduleEntry entry)
        {
            return entry.destination != null ? entry.destination.transform.position : transform.position;
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
        public void SetVisible(bool isVisible)
        {
            GetComponent<SpriteRenderer>().enabled = isVisible;
        }


        private void OnCollisionEnter2D(Collision2D collision)
        {
            collision.gameObject.TryGetComponent<HouseLadderZone>(out var ladder);
            if (ladder != null)
            {
                // NPC во входной зоне лестницы
                CurrentFloor += ladder.TargetFloor;
                ladder.floorManager.ChangeFloor(ladder.TargetFloor);
            }

        }

    }
    [System.Serializable]
    public class ScheduleEntry
    {
        [Range(0, 23)] public int hour;
        [Range(0, 59)] public int minute;
        public DayOfWeek day;
        public NPCActivityType activity;
        public NavTargetPoint destination;
    }



}