using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.NPC.States
{
    public class GoToLocationState : INPCState
    {
        private readonly NPCController npc;
        private readonly NavTargetPoint target;
        private readonly Action onReachedDestination;

        private Queue<Vector3> path;
        private bool followingPath;

        public GoToLocationState(NPCController npc, NavTargetPoint target, Action onReachedDestination = null)
        {
            this.npc = npc;
            this.target = target;
            this.onReachedDestination = onReachedDestination;
        }

        public void Enter()
        {
            path = BuildPathToTarget();
            MoveToNextStep();
        }

        public void Update()
        {
            if (!followingPath)
                return;

            if (!npc.Agent.pathPending && npc.Agent.remainingDistance <= npc.Agent.stoppingDistance)
            {
                npc.Agent.ResetPath();
                MoveToNextStep();
            }
        }

        public void Exit()
        {
            npc.Animator.SetFloat("Speed", 0f);
            npc.Agent.ResetPath();
        }

        private void MoveToNextStep()
        {
            if (path.Count == 0)
            {
                followingPath = false;
                npc.Animator.SetFloat("Speed", 0f);
                npc.CurrentFloor = target.floorIndex; // Обновляем текущий этаж NPC
                npc.CurrentHouse = target.house; // Обновляем текущий дом NPC
                onReachedDestination?.Invoke();
                //npc.StateMachine.ChangeState(new IdleState(npc));
                return;
            }

            var nextPoint = path.Dequeue();
            npc.Agent.SetDestination(nextPoint);
            npc.Animator.SetFloat("Speed", 1f);
            followingPath = true;
        }

        private Queue<Vector3> BuildPathToTarget()
        {
            var result = new Queue<Vector3>();

            //// Если нужно войти в другой дом
            //if (target.IsInsideHouse && npc.CurrentHouse != target.house)
            //{
            //    // Идем ко входу дома
            //    Vector3 entrance = HouseEntranceRegistry.GetEntranceFor(target.house); // <-- здесь используется централизованный метод
            //    result.Enqueue(entrance);
            //}

            // Если нужно подняться/спуститься на другой этаж в том же доме
            if (npc.CurrentFloor != target.floorIndex)
            {
                var ladderSteps = LadderPathfinder.FindPath(npc.CurrentFloor, target.floorIndex, target.house);
                foreach (var point in ladderSteps)
                    result.Enqueue(point);
            }

            // Конечная цель
            result.Enqueue(target.transform.position);
            return result;
        }
    }
}
