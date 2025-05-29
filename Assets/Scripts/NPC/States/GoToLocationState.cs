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
                Debug.LogWarning($"[GoToLocationState] NPC {npc.name}: ������� ��������� �� ������� ����!");
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

            // === NPC ������� � ���� ������ ===
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

            // === NPC ������ � ���� ������� ===
            else if (npcInside && !targetInside)
            {
                if (npc.CurrentFloor != npc.CurrentHouse.entranceFloor)
                {
                    var ladderPath = LadderPathfinder.FindPath(npc.CurrentFloor, npc.CurrentHouse.entranceFloor, npc.CurrentHouse);

                    if (ladderPath == null || ladderPath.Count == 0)
                    {
                        Debug.LogWarning($"[GoToLocationState] NPC {npc.name}: ���� ���� �� ������ �� {npc.CurrentFloor} ����� � ������ �� ���� {npc.CurrentHouse.name}");
                        return new Queue<Vector3>(); // ��������� � �� ���������� ���� � ����� ���������
                    }

                    foreach (var step in ladderPath)
                        path.Enqueue(step);
                }

                path.Enqueue(npc.CurrentHouse.GetEntrancePosition());
                path.Enqueue(target.transform.position);
            }

            // === NPC � ���� � ������ ����� ===
            else if (npcInside && targetInside && npc.CurrentHouse != target.house)
            {
                // ����� �� �������� ����
                if (npc.CurrentFloor != npc.CurrentHouse.entranceFloor)
                {
                    var downPath = LadderPathfinder.FindPath(npc.CurrentFloor, npc.CurrentHouse.entranceFloor, npc.CurrentHouse);
                    foreach (var step in downPath)
                        path.Enqueue(step);
                }

                path.Enqueue(npc.CurrentHouse.GetEntrancePosition());

                // ����� � ����� ���
                path.Enqueue(target.house.GetEntrancePosition());

                if (target.floorIndex != target.house.entranceFloor)
                {
                    var upPath = LadderPathfinder.FindPath(target.house.entranceFloor, target.floorIndex, target.house);
                    foreach (var step in upPath)
                        path.Enqueue(step);
                }

                path.Enqueue(target.transform.position);
            }

            // === NPC � ���� � ����� ����, �� �� ������ ������ ===
            else if (npcInside && targetInside && npc.CurrentHouse == target.house && npc.CurrentFloor != target.floorIndex)
            {
                var sameHousePath = LadderPathfinder.FindPath(npc.CurrentFloor, target.floorIndex, npc.CurrentHouse);
                foreach (var step in sameHousePath)
                    path.Enqueue(step);

                path.Enqueue(target.transform.position);
            }

            // === �� ����� ����� ===
            else
            {
                path.Enqueue(target.transform.position);
            }

            return path;
        }
    }
}
