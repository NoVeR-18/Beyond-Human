using System;
using UnityEngine;

namespace Assets.Scripts.NPC.States
{

    public class GoToLocationState : INPCState
    {
        private NPCController npc;
        private Vector3 destination;
        private Action onReachedDestination;

        public GoToLocationState(NPCController npc, Vector3 destination, Action onReachedDestination = null)
        {
            this.npc = npc;
            this.destination = destination;
            this.onReachedDestination = onReachedDestination;
        }

        public void Enter()
        {
            npc.Agent.SetDestination(destination);
            npc.Animator.SetFloat("Speed", 1f);
        }

        public void Update()
        {
            if (!npc.Agent.pathPending && npc.Agent.remainingDistance <= npc.Agent.stoppingDistance)
            {
                npc.Agent.ResetPath();
                npc.Animator.SetFloat("Speed", 0f);

                if (onReachedDestination != null)
                {
                    onReachedDestination.Invoke();
                }
                else
                {
                    npc.StateMachine.ChangeState(new IdleState(npc)); // или состояние ожидания
                }
            }
        }

        public void Exit()
        {
            npc.Animator.SetFloat("Speed", 0f);
        }
    }

}