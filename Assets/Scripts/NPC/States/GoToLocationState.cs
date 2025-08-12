using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Assets.Scripts.NPC.States
{
    public class GoToLocationState : INPCState
    {
        private NPCController npc;
        private NavTargetPoint target;
        private Action onReachedDestination;

        private Queue<Vector3> currentPath;

        private const float DuplicateEpsilon = 0.1f;
        private const float NavSampleRadius = 1f;

        public GoToLocationState(NPCController npc, NavTargetPoint destination, Action onReachedDestination = null)
        {
            this.npc = npc ?? throw new ArgumentNullException(nameof(npc));
            this.target = destination ?? throw new ArgumentNullException(nameof(destination));
            this.onReachedDestination = onReachedDestination;
        }

        public void Enter()
        {
            currentPath = BuildPathConsideringFloors();

            if (currentPath == null || currentPath.Count == 0)
            {
                Debug.LogWarning($"[GoToLocationState] NPC {npc.name}: путь пуст или не удалось построить маршрут — переключаемся в Idle.");
                npc.StateMachine.ChangeState(new IdleState(npc));
                return;
            }

            FollowNextInPath();
        }

        public void Update()
        {
            if (npc.Agent == null) return;

            // Если агент не в процессе построения пути и дошёл до текущей цели
            if (!npc.Agent.pathPending && npc.Agent.remainingDistance <= npc.Agent.stoppingDistance)
            {
                if (currentPath != null && currentPath.Count > 0)
                {
                    FollowNextInPath();
                }
                else
                {
                    // Финальное завершение — агент достиг конечной точки
                    npc.Agent.ResetPath();
                    npc.Animator?.SetFloat("Speed", 0f);

                    // Обновляем информацию о текущем доме/этаже — здесь как в оригинале
                    npc.CurrentHouse = target.house;
                    npc.CurrentFloor = target.floorIndex;

                    onReachedDestination?.Invoke();
                }
            }
        }

        public void Exit()
        {
            npc.Animator?.SetFloat("Speed", 0f);
        }

        private void FollowNextInPath()
        {
            if (currentPath == null || currentPath.Count == 0)
            {
                Debug.LogWarning($"[GoToLocationState] NPC {npc.name}: попытка следовать по пустому пути!");
                npc.StateMachine.ChangeState(new IdleState(npc));
                return;
            }

            bool destinationSet = false;

            // Пытаемся найти первую доступную точку на NavMesh
            while (currentPath.Count > 0)
            {
                var next = currentPath.Dequeue();

                if (NavMesh.SamplePosition(next, out NavMeshHit hit, NavSampleRadius, NavMesh.AllAreas))
                {
                    npc.Agent.SetDestination(hit.position);
                    npc.Animator?.SetFloat("Speed", 1f);
                    destinationSet = true;
                    break;
                }
                else
                {
                    Debug.LogWarning($"[GoToLocationState] NPC {npc.name}: точка {next} недоступна на NavMesh — пропускаем.");
                }
            }

            if (!destinationSet)
            {
                // Нигде двигаться — безопасно завершаем состояние
                Debug.LogWarning($"[GoToLocationState] NPC {npc.name}: ни одна точка не доступна — завершаем движение.");
                npc.Agent.ResetPath();
                npc.Animator?.SetFloat("Speed", 0f);
                npc.StateMachine.ChangeState(new IdleState(npc));
            }
        }

        private Queue<Vector3> BuildPathConsideringFloors()
        {
            var steps = new List<Vector3>();

            bool npcInside = npc.CurrentHouse != null;
            bool targetInside = target.IsInsideHouse;

            void AddStep(Vector3 pos)
            {
                if (steps.Count == 0 || Vector3.Distance(steps[steps.Count - 1], pos) > DuplicateEpsilon)
                    steps.Add(pos);
            }

            // === NPC снаружи и цель внутри ===
            if (!npcInside && targetInside)
            {
                AddStep(target.house.GetEntrancePosition());

                if (target.floorIndex != target.house.entranceFloor)
                {
                    var ladderPath = LadderPathfinder.FindPath(target.house.entranceFloor, target.floorIndex, target.house);
                    if (ladderPath == null || ladderPath.Count == 0)
                    {
                        Debug.LogWarning($"[GoToLocationState] NPC {npc.name}: путь вверх в дом {target.house.name} не найден (входной этаж -> {target.floorIndex}).");
                        return new Queue<Vector3>(); // прерываем, чтобы не зависнуть
                    }

                    foreach (var step in ladderPath)
                        AddStep(step);
                }

                AddStep(target.transform.position);
            }

            // === NPC внутри и цель снаружи ===
            else if (npcInside && !targetInside)
            {
                if (npc.CurrentFloor != npc.CurrentHouse.entranceFloor)
                {
                    var ladderPath = LadderPathfinder.FindPath(npc.CurrentFloor, npc.CurrentHouse.entranceFloor, npc.CurrentHouse);
                    if (ladderPath == null || ladderPath.Count == 0)
                    {
                        Debug.LogWarning($"[GoToLocationState] NPC {npc.name}: путь вниз до выхода из дома {npc.CurrentHouse.name} не найден.");
                        return new Queue<Vector3>();
                    }

                    foreach (var step in ladderPath)
                        AddStep(step);
                }

                AddStep(npc.CurrentHouse.GetEntrancePosition());
                AddStep(target.transform.position);
            }

            // === NPC и цель в разных домах ===
            else if (npcInside && targetInside && npc.CurrentHouse != target.house)
            {
                // Выйти из текущего дома
                if (npc.CurrentFloor != npc.CurrentHouse.entranceFloor)
                {
                    var downPath = LadderPathfinder.FindPath(npc.CurrentFloor, npc.CurrentHouse.entranceFloor, npc.CurrentHouse);
                    if (downPath == null || downPath.Count == 0)
                    {
                        Debug.LogWarning($"[GoToLocationState] NPC {npc.name}: путь вниз не найден из {npc.CurrentFloor} этажа к выходу из дома {npc.CurrentHouse.name}");
                        return new Queue<Vector3>();
                    }

                    foreach (var step in downPath)
                        AddStep(step);
                }

                AddStep(npc.CurrentHouse.GetEntrancePosition());

                // Зайти в новый дом
                AddStep(target.house.GetEntrancePosition());

                if (target.floorIndex != target.house.entranceFloor)
                {
                    var upPath = LadderPathfinder.FindPath(target.house.entranceFloor, target.floorIndex, target.house);
                    if (upPath == null || upPath.Count == 0)
                    {
                        Debug.LogWarning($"[GoToLocationState] NPC {npc.name}: путь вверх в дом {target.house.name} не найден.");
                        return new Queue<Vector3>();
                    }

                    foreach (var step in upPath)
                        AddStep(step);
                }

                AddStep(target.transform.position);
            }

            // === NPC и цель в одном доме, но на разных этажах ===
            else if (npcInside && targetInside && npc.CurrentHouse == target.house && npc.CurrentFloor != target.floorIndex)
            {
                var sameHousePath = LadderPathfinder.FindPath(npc.CurrentFloor, target.floorIndex, npc.CurrentHouse);
                if (sameHousePath == null || sameHousePath.Count == 0)
                {
                    Debug.LogWarning($"[GoToLocationState] NPC {npc.name}: внутренний путь между этажами не найден (из {npc.CurrentFloor} в {target.floorIndex}).");
                    return new Queue<Vector3>();
                }

                foreach (var step in sameHousePath)
                    AddStep(step);

                AddStep(target.transform.position);
            }

            // === На одном этаже или оба снаружи ===
            else
            {
                AddStep(target.transform.position);
            }

            // Логируем путь для дебага
            var sb = new System.Text.StringBuilder();
            for (int i = 0; i < steps.Count; i++)
            {
                sb.Append(steps[i].ToString());
                if (i < steps.Count - 1) sb.Append(" -> ");
            }
            Debug.Log($"[GoToLocationState] NPC {npc.name}: построен путь: {sb}");

            return new Queue<Vector3>(steps);
        }
    }
}
