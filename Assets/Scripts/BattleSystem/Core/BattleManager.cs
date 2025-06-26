using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BattleSystem
{
    public class BattleManager : MonoBehaviour
    {
        public List<BattleCharacter> teamA = new();
        public List<BattleCharacter> teamB = new();
        public List<BattleCharacter> teamC = new();

        [Header("Timing")]
        public float actionInterval = 1f;

        [Header("UI")]
        public AbilityBarUI playerAbilityUI;
        public AbilityBarUI Team2AbilityUI;
        public AbilityBarUI Team3AbilityUI;
        public StatusEffectPanel playerEffectUI;
        public StatusEffectPanel Team2EffectUI;
        public StatusEffectPanel Team3EffectUI;



        private Dictionary<BattleTeam, List<BattleSpawnPoint>> spawnPoints;
        private Dictionary<BattleSpawnPoint, BattleCharacter> occupiedSpawns = new();

        private void Start()
        {
            var context = BattleContext.Instance;

            if (context == null)
            {
                Debug.LogError("No BattleContext!");
                return;
            }

            spawnPoints = FindObjectsOfType<BattleSpawnPoint>()
        .GroupBy(sp => sp.team)
        .ToDictionary(g => g.Key, g => g.OrderBy(sp => sp.index).ToList());

            SpawnParticipantsFromSetup();
            Init();
            StartCoroutine(BattleLoop());
        }

        private void SpawnParticipantsFromSetup()
        {
            var teamDict = new Dictionary<BattleTeam, List<BattleCharacter>> {
        { BattleTeam.Team1, teamA },
        { BattleTeam.Team2, teamB },
        { BattleTeam.Team3, teamC }
    };

            foreach (var unit in BattleContext.Instance.Participants)
            {
                if (!spawnPoints.ContainsKey(unit.team))
                {
                    Debug.LogWarning($"No spawn points for team {unit.team}");
                    continue;
                }

                var index = teamDict[unit.team].Count;
                var spawnPoint = spawnPoints[unit.team].ElementAtOrDefault(index);
                if (spawnPoint == null)
                {
                    Debug.LogWarning($"Not enough spawn points for team {unit.team}");
                    continue;
                }

                var go = Instantiate(unit.prefab, spawnPoint.transform.position, Quaternion.identity);
                var bc = go.GetComponent<BattleCharacter>();

                bc.Team = unit.team;
                bc.CurrentStats = unit.stats;
                bc.Abilities = unit.abilities;

                teamDict[unit.team].Add(bc);
            }

            BattleContext.Instance.Participants.Clear();

        }
        public BattleSpawnPoint GetFreeSpawnPoint(BattleTeam team)
        {
            if (!spawnPoints.ContainsKey(team)) return null;

            foreach (var point in spawnPoints[team])
            {
                if (!occupiedSpawns.ContainsKey(point) || occupiedSpawns[point] == null || !occupiedSpawns[point].IsAlive)
                    return point;
            }

            return null;
        }
        private void Init()
        {
            foreach (var character in teamA.Concat(teamB))
            {
                if (character.Team == BattleTeam.Team1 && playerAbilityUI != null)
                {
                    playerAbilityUI.Setup(character.Abilities, character);
                    character.abilityUI = playerAbilityUI;
                    character.statusEffectPanel = playerEffectUI;
                }
                else if (character.Team == BattleTeam.Team2 && Team2AbilityUI != null)
                {
                    Team2AbilityUI.Setup(character.Abilities, character);
                    character.abilityUI = Team2AbilityUI;
                    character.statusEffectPanel = Team2EffectUI;
                }
                else if (character.Team == BattleTeam.Team2 && Team3AbilityUI != null)
                {
                    Team3AbilityUI.Setup(character.Abilities, character);
                    character.abilityUI = Team3AbilityUI;
                    character.statusEffectPanel = Team3EffectUI;
                }
            }
        }

        private bool IsBattleOver()
        {
            return teamA.All(c => !c.IsAlive) || (teamB.All(c => !c.IsAlive) && (teamC.All(c => !c.IsAlive)));
        }

        private List<BattleCharacter> GetAllCharacters()
        {
            return teamA.Concat(teamB).Concat(teamC).ToList();
        }

        private List<BattleCharacter> SelectTargets(BattleCharacter caster, AbilityTargetType targetType)
        {
            List<BattleCharacter> allies = GetAllCharacters().Where(c => c.Team == caster.Team && c.IsAlive).ToList();
            List<BattleCharacter> enemies = GetAllCharacters().Where(c => c.Team != caster.Team && c.IsAlive).ToList();

            switch (targetType)
            {
                case AbilityTargetType.SingleEnemy:
                    return GetRandomAliveTarget(enemies.Where(x => x.IsAlive).ToList());
                case AbilityTargetType.SingleAlly:
                    var possibleAllies = allies.Where(x => x.IsAlive && x != caster).ToList();
                    return GetRandomAliveTarget(possibleAllies.Count > 0 ? possibleAllies : new List<BattleCharacter> { caster });
                case AbilityTargetType.Self:
                    return new List<BattleCharacter> { caster };
                case AbilityTargetType.AllEnemies:
                    return enemies.Where(x => x.IsAlive).ToList();
                case AbilityTargetType.AllAllies:
                    return allies.Where(x => x.IsAlive).ToList();
                case AbilityTargetType.All:
                    return GetAllCharacters().Where(x => x.IsAlive).ToList();
                default:
                    return new();
            }
        }

        private List<BattleCharacter> GetRandomAliveTarget(List<BattleCharacter> list)
        {
            if (list.Count == 0) return new();
            return new List<BattleCharacter> { list[Random.Range(0, list.Count)] };
        }

        private void ApplyAbility(BattleCharacter caster, BattleCharacter target, AbilityData ability)
        {
            if (target == null || !target.IsAlive) return;

            if (ability.abilityType == AbilityType.Magical && !caster.CanUseMagic()) return;
            if (ability.abilityType == AbilityType.Physical && !caster.CanUsePhysical()) return;

            int totalDamage = ability.baseDamage;
            if (ability.abilityType == AbilityType.Magical)
                totalDamage = Mathf.RoundToInt(totalDamage * caster.GetMagicDamageMultiplier());
            if (ability.abilityType == AbilityType.Physical)
                totalDamage = Mathf.RoundToInt(totalDamage * caster.GetPhysicalDamageMultiplier());
            foreach (var effect in target.GetCurrentEffects())
            {
                if (effect.weaknessToDamageType == ability.damageType)
                {
                    totalDamage = Mathf.RoundToInt(totalDamage * effect.weaknessMultiplier);
                }
            }

            if (ability.effects != null && ability.effects.Count > 0)
                target.ApplyStatusEffect(ability.effects);

            if (totalDamage > 0)
                target.TakeDamage(totalDamage);

            Debug.Log($"{caster.characterName} used {ability.abilityName} on {target.characterName}, dealing {totalDamage} damage!");

            if (ability.summonPrefab != null)
            {
                var freePoint = GetFreeSpawnPoint(caster.Team);
                if (freePoint == null)
                {
                    Debug.LogWarning($"Нет места для призыва существа {ability.abilityName} — способность пропущена");
                    return; // вместо применения способности — ничего не делаем
                }

                var summon = Instantiate(ability.summonPrefab, freePoint.transform.position, Quaternion.identity);
                var battleChar = summon.GetComponent<BattleCharacter>();
                if (battleChar != null)
                {
                    battleChar.Team = caster.Team;
                    if (caster.Team == BattleTeam.Team1)
                        teamA.Add(battleChar);
                    else if (caster.Team == BattleTeam.Team2)
                        teamB.Add(battleChar);
                    else
                        teamC.Add(battleChar);

                    occupiedSpawns[freePoint] = battleChar;
                }

            }
        }
        private void CleanDeadCharactersAndFreeSpawns()
        {
            void CleanTeam(List<BattleCharacter> team)
            {
                for (int i = team.Count - 1; i >= 0; i--)
                {
                    var character = team[i];
                    if (!character.IsAlive)
                    {
                        FreeSpawnIfOccupied(character);
                        team.RemoveAt(i);
                        Destroy(character.gameObject); // или можно делать return to pool
                    }
                }
            }

            CleanTeam(teamA);
            CleanTeam(teamB);
            if (teamC != null) CleanTeam(teamC);
        }

        private void FreeSpawnIfOccupied(BattleCharacter character)
        {
            if (occupiedSpawns == null) return;

            var spawn = occupiedSpawns.FirstOrDefault(kv =>
                kv.Value == character).Key;

            if (spawn != null)
            {
                occupiedSpawns.Remove(spawn);
            }
        }

        private IEnumerator BattleLoop()
        {
            while (true)
            {
                CleanDeadCharactersAndFreeSpawns();

                if (IsBattleOver())
                    break;
                foreach (var character in GetAllCharacters())
                {
                    if (character.IsAlive)
                    {
                        character.StatusEffectTick(actionInterval);
                    }
                }

                yield return ProcessTeamTurn(teamA);
                yield return ProcessTeamTurn(teamB);
                yield return ProcessTeamTurn(teamC);

                yield return new WaitForSeconds(actionInterval);
            }

            Debug.Log("Battle Over!");
        }

        private IEnumerator ProcessTeamTurn(List<BattleCharacter> team)
        {
            List<BattleCharacter> turnOrder = new(team);

            foreach (var character in turnOrder)
            {
                if (!character.IsAlive || !character.CanAct())
                    continue;

                AbilityData ability = character.GetNextReadyAbility(this);
                if (ability == null) continue;

                List<BattleCharacter> targets = SelectTargets(character, ability.TargetType);
                if (targets == null || targets.Count == 0) continue;

                if (ability.castTime > 0)
                    yield return new WaitForSeconds(ability.castTime * character.GetCastSpeedMultiplier());

                foreach (var target in targets)
                {
                    ApplyAbility(character, target, ability);
                }

                character.PlayAttackAnimation(ability.animationTrigger);
                character.StartCooldown(ability);

                team.Remove(character);
                team.Add(character);

                yield break;
            }

            yield return new WaitForSeconds(actionInterval);
        }

        private void Update()
        {
            UpdateAllCooldowns(Time.deltaTime);
        }

        private void UpdateAllCooldowns(float deltaTime)
        {
            foreach (var character in teamA.Concat(teamB))
            {
                character.TickCooldowns(deltaTime);
            }

            if (playerAbilityUI != null)
            {
                foreach (var character in teamA)
                {
                    playerAbilityUI.UpdateCooldowns(character.GetCooldowns(), character);
                    playerEffectUI.UpdateStatusEffects(character.GetStatusEffects(), character);
                }
            }

            if (Team2AbilityUI != null)
            {
                foreach (var character in teamB)
                {
                    Team2AbilityUI.UpdateCooldowns(character.GetCooldowns(), character);
                    Team2EffectUI.UpdateStatusEffects(character.GetStatusEffects(), character);
                }
            }
            if (Team3AbilityUI != null)
            {
                foreach (var character in teamC)
                {
                    Team3AbilityUI.UpdateCooldowns(character.GetCooldowns(), character);
                    Team3EffectUI.UpdateStatusEffects(character.GetStatusEffects(), character);
                }
            }
        }
    }
}
