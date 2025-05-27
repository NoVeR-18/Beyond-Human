using UnityEngine;

namespace Assets.Scripts.NPC.States
{
    public class HiddenState : INPCState
    {
        private NPCController npc;
        private Renderer[] renderers;
        private Collider2D[] colliders;
        private MonoBehaviour[] componentsToDisable;

        public HiddenState(NPCController npc)
        {
            this.npc = npc;
        }

        public void Enter()
        {
            // Отключаем визуал
            renderers = npc.GetComponentsInChildren<Renderer>();
            foreach (var r in renderers)
                r.enabled = false;

            // Отключаем коллайдеры
            colliders = npc.GetComponentsInChildren<Collider2D>();
            foreach (var c in colliders)
                c.enabled = false;

            // Отключаем активные компоненты, например AI, Agent и т.д.
            componentsToDisable = npc.GetComponentsInChildren<MonoBehaviour>();
            foreach (var comp in componentsToDisable)
            {
                if (comp != npc) // не трогаем себя
                    comp.enabled = false;
            }

            // Можно логировать
            Debug.Log($"{npc.name} скрыт.");
        }

        public void Exit()
        {
            // Возвращаем визуал
            foreach (var r in renderers)
                r.enabled = true;
        }
        public void Update()
        {

        }
    }
}

// Возвращаем коллайдеры
