using NPCEnums;

namespace Assets.Scripts.NPC.States
{

    using UnityEngine;

    namespace Assets.Scripts.NPC.States
    {
        public class GuardState : INPCState, IInterruptible
        {
            private NPCController npc;
            private Vector3 guardPoint;
            private float idleLookAroundTimer;
            private float idleLookAroundInterval = 3f;
            private float patrolRadius = 2f;
            private float patrolWaitTime = 2f;
            private float patrolTimer;
            private Vector3 patrolTarget;
            private bool hasPoint;

            public GuardState(NPCController npc, Transform guardPoint = null)
            {
                this.npc = npc;
                if (guardPoint == null)
                {
                    hasPoint = false;
                }
                else
                {
                    hasPoint = true;
                    this.guardPoint = guardPoint.position;
                }
            }

            public void Enter()
            {
                npc.Agent.ResetPath();
                if (hasPoint)
                {
                    npc.Agent.SetDestination(guardPoint);
                }
                else
                {
                    patrolTimer = 0;
                    SetNewPatrolPoint();
                }
            }

            public void Exit()
            {
                npc.Agent.ResetPath();
            }

            public void Update()
            {
                //// Реакция на игрока
                //if (npc.CanSeePlayer(out var player))
                //{
                //    npc.target = player;
                //    npc.StateMachine.ChangeState(new ChaseState(npc, InterruptReason.ChaseAlert));
                //    return;
                //}

                if (hasPoint)
                {
                    float dist = Vector3.Distance(npc.transform.position, guardPoint);
                    if (dist > 0.5f)
                    {
                        npc.Agent.SetDestination(guardPoint);
                    }
                    else
                    {
                        npc.Agent.ResetPath();
                        // Можно добавить Idle-анимацию и периодическое "осматривание"
                        idleLookAroundTimer -= Time.deltaTime;
                        if (idleLookAroundTimer <= 0)
                        {
                            // Анимация или поворот
                            idleLookAroundTimer = idleLookAroundInterval;
                        }
                    }
                }
                else
                {
                    // Примитивное патрулирование
                    if (!npc.Agent.pathPending && npc.Agent.remainingDistance < 0.5f)
                    {
                        patrolTimer -= Time.deltaTime;
                        if (patrolTimer <= 0)
                        {
                            SetNewPatrolPoint();
                        }
                    }
                }
            }

            private void SetNewPatrolPoint()
            {
                Vector2 randomPoint = Random.insideUnitCircle * patrolRadius;
                patrolTarget = npc.transform.position + new Vector3(randomPoint.x, randomPoint.y, 0);
                npc.Agent.SetDestination(patrolTarget);
                patrolTimer = patrolWaitTime;
            }

            public void Interrupt(NPCController source, InterruptReason reason)
            {
                if (reason == InterruptReason.ScreamHelp || reason == InterruptReason.ChaseAlert)
                {
                    npc.target = source.transform;
                    npc.StateMachine.ChangeState(new ChaseState(npc, InterruptReason.ChaseAlert));
                }
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
        }
    }


}
