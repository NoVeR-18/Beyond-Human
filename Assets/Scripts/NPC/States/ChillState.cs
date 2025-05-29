using NPCEnums;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.NPC.States
{
    public class ChillState : INPCState
    {
        private readonly NPCController npc;
        private Vector3 centerPoint;
        private float chillRadius = 3f;
        private float waitTime = 2f;
        private bool isWaiting;

        public ChillState(NPCController npc, Vector3? center = null, float radius = 1.5f)
        {
            this.npc = npc;
            centerPoint = center ?? npc.transform.position;
            chillRadius = radius;
        }

        public void Enter()
        {

            npc.StartContextDialogue(DialogueContext.Idle);
            MoveToNextPoint();
        }

        public void Exit()
        {
            npc.Agent.ResetPath();
        }

        public void Update()
        {
            if (isWaiting || npc.Agent.pathPending || npc.Agent.remainingDistance > 0.2f)
                return;

            npc.Animator?.SetFloat("Speed", 0);
            npc.StartCoroutine(WaitAndMove());
        }

        private IEnumerator WaitAndMove()
        {
            isWaiting = true;
            yield return new WaitForSeconds(waitTime);
            MoveToNextPoint();
            isWaiting = false;
        }

        private void MoveToNextPoint()
        {
            Vector2 offset = Random.insideUnitCircle * chillRadius;
            Vector3 target = centerPoint + new Vector3(offset.x, offset.y, 0);
            npc.Agent.SetDestination(target);
            npc.Animator?.SetFloat("Speed", 1);
        }
    }
}
