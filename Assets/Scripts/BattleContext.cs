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
    private Dictionary<FactionData, BattleTeam> factionToTeam = new();
    private int teamIndex = 1;

    private bool joining = false;

    [SerializeField] private bool autoStartForTesting = false;
    public List<BattleParticipantData> battleParticipantDatas = new List<BattleParticipantData>();
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject); // не даём пересоздать
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        if (autoStartForTesting)
        {
            foreach (var data in battleParticipantDatas)
            {
                Charackters.Add(data);
            }

        }
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
        if (npc.battleParticipantData == null || npc.battleParticipantData.faction == null) return;

        var faction = npc.battleParticipantData.faction;

        // === Пробуем добавить в список участников ===
        if (!Charackters.Add(npc.battleParticipantData))
        {
            return; // уже есть — выходим, не дублируем
        }

        // === Назначаем команду (если нужно) ===
        if (!factionToTeam.TryGetValue(faction, out var team))
        {
            if (factionToTeam.Count == 0)
            {
                team = (BattleTeam)teamIndex++;
                factionToTeam.Add(faction, team);
            }
            else
            {
                bool foundEnemy = false;

                foreach (var kvp in factionToTeam)
                {
                    var otherFaction = kvp.Key;
                    var rel = faction.GetAttitudeTowards(otherFaction);
                    var reverseRel = otherFaction.GetAttitudeTowards(faction);

                    if (rel < 0 || reverseRel < 0)
                    {
                        foundEnemy = true;
                        break;
                    }
                }

                if (!foundEnemy)
                {
                    Debug.Log($"{faction.name} дружественен всем, не вступает в бой.");
                    Charackters.Remove(npc.battleParticipantData); // ❗ убираем из боевой группы
                    NotifySpreadComplete();
                    return;
                }

                if (factionToTeam.Count >= 3)
                {
                    Debug.LogWarning($"Слишком много фракций в бою — {faction.name} не добавлен.");
                    Charackters.Remove(npc.battleParticipantData); // ❗ убираем из боевой группы
                    NotifySpreadComplete();
                    return;
                }

                team = (BattleTeam)teamIndex++;
                factionToTeam.Add(faction, team);
            }
        }

        // Присваиваем боевую команду
        npc.battleParticipantData.team = team;

        activeSpreaders++;

        var interruptible = npc.StateMachine.CurrentState as IInterruptible;
        interruptible?.Interrupt(npc, InterruptReason.CombatJoin);
    }


    public void AddPlayer(PlayerController npc)
    {
        if (!joining) return;
        var playerTeam = PartyManager.Instance.CharacterToBattleParticiant();
        foreach (var particiant in playerTeam)
        {
            if (Charackters.Add(particiant))
            {
                particiant.team = BattleTeam.Team1; // Присваиваем команду игрока
            }
            else
            {
                Debug.LogWarning($"Игрок {particiant.nameID} уже в бою, не добавляем повторно.");
            }
        }

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
        teamIndex = 1;
        Debug.Log($"Бой начинается! Участников: {Charackters.Count}");
        returnSceneName = SceneManager.GetActiveScene().name;
        SaveSystem.Instance.SaveAll();
        SceneManager.LoadScene("BattleScene");
        //SceneManager.LoadScene("BattleScene");
    }
}


[System.Serializable]
public class BattleParticipantData
{
    public BattleCharacter battleCharacter;
    [HideInInspector] public string nameID;
    public BattleTeam team;
    public CharacterStats stats;
    public List<AbilityData> abilities = new();

    [HideInInspector]
    public FactionData faction;
}
