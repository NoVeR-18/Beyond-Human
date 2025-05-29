using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.NPC.States
{
    public class GoToLocationState : INPCState
    {
        private NPCController npc;
        private NavTargetPoint target;
        private Action onReachedDestination;

        private Queue<Vector3> currentPath;

        public GoToLocationState(NPCController npc, NavTargetPoint destination, Action onReachedDestination = null)
        {
            this.npc = npc;
            this.target = destination;
            this.onReachedDestination = onReachedDestination;
        }

        public void Enter()
        {
            currentPath = BuildPathConsideringFloors();
            FollowNextInPath();
        }

        public void Update()
        {
            if (!npc.Agent.pathPending && npc.Agent.remainingDistance <= npc.Agent.stoppingDistance)
            {
                if (currentPath.Count > 0)
                {
                    FollowNextInPath();
                }
                else
                {
                    npc.Agent.ResetPath();
                    npc.Animator.SetFloat("Speed", 0f);
                    npc.CurrentHouse = target.house;
                    npc.CurrentFloor = target.floorIndex;
                    onReachedDestination?.Invoke();
                }
            }
        }

        public void Exit()
        {
            npc.Animator.SetFloat("Speed", 0f);
        }

        private void FollowNextInPath()
        {
            if (currentPath == null || currentPath.Count == 0)
            {
                Debug.LogWarning($"[GoToLocationState] NPC {npc.name}: попытка следовать по пустому пути!");
                npc.StateMachine.ChangeState(new IdleState(npc));
                return;
            }

            Vector3 next = currentPath.Dequeue();
            npc.Agent.SetDestination(next);
            npc.Animator.SetFloat("Speed", 1f);
        }

        private Queue<Vector3> BuildPathConsideringFloors()
        {
            var path = new Queue<Vector3>();

            bool npcInside = npc.CurrentHouse != null;
            bool targetInside = target.IsInsideHouse;

            // === NPC снаружи и цель внутри ===
            if (!npcInside && targetInside)
            {
                path.Enqueue(target.house.GetEntrancePosition());
                npc.CurrentHouse = target.house;
                if (target.floorIndex != target.house.entranceFloor)
                {
                    var ladderPath = LadderPathfinder.FindPath(target.house.entranceFloor, target.floorIndex, target.house);
                    foreach (var step in ladderPath)
                        path.Enqueue(step);
                }

                path.Enqueue(target.transform.position);
            }

            // === NPC внутри и цель снаружи ===
            else if (npcInside && !targetInside)
            {
                if (npc.CurrentFloor != npc.CurrentHouse.entranceFloor)
                {
                    var ladderPath = LadderPathfinder.FindPath(npc.CurrentFloor, npc.CurrentHouse.entranceFloor, npc.CurrentHouse);

                    if (ladderPath == null || ladderPath.Count == 0)
                    {
                        Debug.LogWarning($"[GoToLocationState] NPC {npc.name}: путь вниз не найден из {npc.CurrentFloor} этажа к выходу из дома {npc.CurrentHouse.name}");
                        return new Queue<Vector3>(); // Прерываем и не продолжаем путь — иначе зависание
                    }

                    foreach (var step in ladderPath)
                        path.Enqueue(step);
                }

                path.Enqueue(npc.CurrentHouse.GetEntrancePosition());
                path.Enqueue(target.transform.position);
            }

            // === NPC и цель в разных домах ===
            else if (npcInside && targetInside && npc.CurrentHouse != target.house)
            {
                // Выйти из текущего дома
                if (npc.CurrentFloor != npc.CurrentHouse.entranceFloor)
                {
                    var downPath = LadderPathfinder.FindPath(npc.CurrentFloor, npc.CurrentHouse.entranceFloor, npc.CurrentHouse);
                    foreach (var step in downPath)
                        path.Enqueue(step);
                }

                path.Enqueue(npc.CurrentHouse.GetEntrancePosition());

                // Зайти в новый дом
                path.Enqueue(target.house.GetEntrancePosition());

                if (target.floorIndex != target.house.entranceFloor)
                {
                    var upPath = LadderPathfinder.FindPath(target.house.entranceFloor, target.floorIndex, target.house);
                    foreach (var step in upPath)
                        path.Enqueue(step);
                }

                path.Enqueue(target.transform.position);
            }

            // === NPC и цель в одном доме, но на разных этажах ===
            else if (npcInside && targetInside && npc.CurrentHouse == target.house && npc.CurrentFloor != target.floorIndex)
            {
                var sameHousePath = LadderPathfinder.FindPath(npc.CurrentFloor, target.floorIndex, npc.CurrentHouse);
                foreach (var step in sameHousePath)
                    path.Enqueue(step);

                path.Enqueue(target.transform.position);
            }

            // === На одном этаже ===
            else
            {
                path.Enqueue(target.transform.position);
            }

            return path;
        }
    }
}
