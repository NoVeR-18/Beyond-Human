using NPCEnums;
using UnityEngine;

namespace Assets.Scripts.NPC.States
{
    public class HuntState : INPCState, IInterruptible
    {
        private NPCController npc;
        private Vector3 center;
        private float wanderRadius = 4f;
        private float idleTimer;

        public HuntState(NPCController npc)
        {
            this.npc = npc;
            center = npc.transform.position; // Базовая точка охоты
        }

        public void Enter()
        {
            MoveToRandomPoint();

            npc.StartContextDialogue(DialogueContext.Hunt);
        }

        public void Exit() { }

        public void Update()
        {
            if (npc.CanSeePlayer(out var player))
            {
                npc.target = player;
                npc.StateMachine.ChangeState(new ChaseState(npc, NPCEnums.InterruptReason.Hunting));
                return;
            }

            if (!npc.Agent.pathPending && npc.Agent.remainingDistance < 0.2f)
            {
                idleTimer -= Time.deltaTime;
                if (idleTimer <= 0)
                {
                    MoveToRandomPoint();
                }
            }
        }
        public void Interrupt(NPCController source, InterruptReason reason)
        {
            if (reason == InterruptReason.CombatJoin)
            {

                Debug.Log("GuardState: CombatJoin interrupt received");
                if (!CanJoin())
                {
                    BattleContext.Instance.NotifySpreadComplete(); // Я ничего не сделал
                    return;
                }

                // Присоединяем себя
                BattleContext.Instance.AddParticipant(npc);

                bool someoneCalled = false;
                // Ищем других
                Collider2D[] hits = Physics2D.OverlapCircleAll(npc.transform.position, 10f);
                foreach (var hit in hits)
                {
                    if (hit.TryGetComponent<NPCController>(out var npc))
                        if (npc != this.npc)
                        {
                            var interruptible = npc.StateMachine.CurrentState as IInterruptible;
                            if (interruptible != null)
                            {
                                if (!BattleContext.Instance.Charackters.Contains(npc.battleParticipantData))
                                {
                                    someoneCalled = true;
                                    interruptible.Interrupt(this.npc, InterruptReason.CombatJoin);
                                }
                            }
                        }
                }
                if (!someoneCalled)
                {
                    BattleContext.Instance.NotifySpreadComplete(); // Я ничего не сделал
                }
            }
        }
        private bool CanJoin()
        {
            var myFaction = npc.battleParticipantData.faction;
            if (myFaction == null) return false;

            // Если уже есть члены моей фракции — обязательно присоединяюсь
            foreach (var p in BattleContext.Instance.Charackters)
            {
                if (p.faction == myFaction)
                    return true;
            }

            // Проверка отношения к уже участвующим
            foreach (var p in BattleContext.Instance.Charackters)
            {
                var rel = myFaction.GetAttitudeTowards(p.faction);
                var reverseRel = p.faction.GetAttitudeTowards(myFaction);

                if (rel < 0 || reverseRel < 0)
                    return true; // враг найден
            }

            // Дружественен всем — не вступаю
            return false;
        }
        private void MoveToRandomPoint()
        {
            Vector2 randomOffset = Random.insideUnitCircle * wanderRadius;
            Vector3 target = center + new Vector3(randomOffset.x, randomOffset.y);
            npc.Agent.SetDestination(target);
            idleTimer = Random.Range(1f, 3f);
        }
    }
}
