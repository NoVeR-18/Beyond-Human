using NPCEnums;
using UnityEngine;

namespace Assets.Scripts.NPC.States
{

    public class ChaseState : INPCState
    {
        private NPCController npc;
        private InterruptReason alertReason;

        public ChaseState(NPCController npc, InterruptReason reason = InterruptReason.None)
        {
            this.npc = npc;
            alertReason = reason;
        }

        public void Enter()
        {


        }

        public void Exit()
        {
            npc.Agent.ResetPath();
        }

        public void Update()
        {
            if (npc.target == null)
            {
                npc.StateMachine.ChangeState(new IdleState(npc));
                return;
            }

            npc.Agent.SetDestination(npc.target.position);
            npc.emitter.Activate(alertReason);

            float distance = Vector3.Distance(npc.transform.position, npc.target.position);
            if (distance <= 1.5f) // Радиус поимки
            {
                Debug.Log("Поймал игрока!");
                npc.Agent.ResetPath();
                BattleContext.Instance.BeginCombat(npc);
                BattleContext.Instance.AddPlayer(npc.target.GetComponent<PlayerController>());
                bool someoneCalled = false;
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

                npc.StateMachine.ChangeState(new IdleState(npc));
                // Здесь можно вставить анимацию, переход в другое состояние и т.п.
            }


            if (!npc.CanSeePlayer(out var seenPlayer))
            {
                if (npc.ScheduleEntry != null)
                    npc.StateMachine.ChangeState(npc.GetCurrentScheduleState()); // Можно сделать GoToLastSeenPositionState
            }
        }
    }

}