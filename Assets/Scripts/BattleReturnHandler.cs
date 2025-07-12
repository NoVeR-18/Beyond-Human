using Assets.Scripts.NPC;
using System.Collections.Generic;
using UnityEngine;

public class BattleReturnHandler : MonoBehaviour
{
    private void Awake()
    {
        if (!string.IsNullOrEmpty(BattleContext.Instance?.returnSceneName))
        {
            foreach (var ids in BattleContext.Instance.npcIDsToRemove)
            {
                SaveSystem.Instance.deadNPCIDs.Add(ids);
            }

            BattleContext.Instance.returnSceneName = null;
            BattleContext.Instance.npcIDsToRemove.Clear();

            // Теперь можно сохранить состояние
            SaveSystem.Instance.SaveAll();
        }
    }


    private void RemoveDeadNPCs(List<int> idsToRemove)
    {
        foreach (var npc in FindObjectsOfType<NPCController>())
        {
            if (idsToRemove.Contains(npc.gameObject.GetInstanceID()))
            {
                Destroy(npc.gameObject);
            }
        }
    }
}
