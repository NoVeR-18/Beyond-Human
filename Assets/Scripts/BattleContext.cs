using Assets.Scripts.NPC;
using BattleSystem;
using NPCEnums;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BattleContext : MonoBehaviour
{
    public HashSet<BattleParticipantData> Charackters = new();
    public string returnSceneName;

    public List<string> npcIDsToRemove; // NPC которых нужно удалить по завершении

    public static BattleContext Instance;

    private int activeSpreaders = 0;

    private bool joining = false;
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject); // не даём пересоздать
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }



    public void BeginCombat(NPCController initiator)
    {
        Charackters.Clear();
        joining = true;
        activeSpreaders = 0;

        AddParticipant(initiator);
    }

    public void AddParticipant(NPCController npc)
    {
        if (!joining) return;
        if (!Charackters.Add(npc.battleParticipantData)) return;

        activeSpreaders++; // этот NPC начнёт звать других
        var interruptible = npc.StateMachine.CurrentState as IInterruptible;
        if (interruptible != null)
        {
            interruptible.Interrupt(npc, InterruptReason.CombatJoin);
        }
    }
    public void AddPlayer(PlayerController npc)
    {
        if (!joining) return;
        Charackters.Add(npc.battleParticipantData);

    }

    public void NotifySpreadComplete()
    {
        activeSpreaders = Mathf.Max(0, activeSpreaders - 1);
        if (activeSpreaders == 0)
        {
            FinishBattle();
        }
    }

    private void FinishBattle()
    {
        joining = false;

        if (Charackters.Count < 2)
        {
            Debug.Log("Бой отменён — недостаточно участников.");
            Charackters.Clear();
            return;
        }

        Debug.Log($"Бой начинается! Участников: {Charackters.Count}");
        returnSceneName = SceneManager.GetActiveScene().name;
        SaveSystem.Instance.SaveAll();
        SceneManager.LoadScene("BattleScene");
    }
}

[System.Serializable]
public class BattleParticipantData
{
    public GameObject prefab;
    [HideInInspector] public string nameID;
    public BattleTeam team;
    public CharacterStats stats;
    public List<AbilityData> abilities = new();
}
