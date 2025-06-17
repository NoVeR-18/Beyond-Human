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

        [Header("Timing")]
        public float actionInterval = 1f; // how often to process character turns

        [Header("UI")]
        public AbilityBarUI playerAbilityUI;
        public AbilityBarUI enemyAbilityUI;
        public DamagePopup damagePopupPrefab;

        private void Start()
        {
            teamA = FindObjectsOfType<BattleCharacter>().Where(c => c.Team == BattleTeam.Player).ToList();
            teamB = FindObjectsOfType<BattleCharacter>().Where(c => c.Team == BattleTeam.Enemy).ToList();
            Init();

            StartCoroutine(BattleLoop());
        }

        private void Init()
        {
            foreach (var character in teamA.Concat(teamB))
            {
                if (character.Team == BattleTeam.Player && playerAbilityUI != null)
                {
                    playerAbilityUI.Setup(character.Abilities);
                    character.abilityUI = playerAbilityUI;
                }
                else if (character.Team == BattleTeam.Enemy && enemyAbilityUI != null)
                {
                    enemyAbilityUI.Setup(character.Abilities);
                    character.abilityUI = enemyAbilityUI;

                }
            }

        }


        private bool IsBattleOver()
        {
            return teamA.All(c => !c.IsAlive) || teamB.All(c => !c.IsAlive);
        }

        private List<BattleCharacter> GetAllCharacters()
        {
            return teamA.Concat(teamB).ToList();
        }

        private BattleCharacter SelectTarget(BattleCharacter caster, AbilityTargetType targetType)
        {
            List<BattleCharacter> possibleTargets = new();

            switch (targetType)
            {
                case AbilityTargetType.SingleEnemy:
                    possibleTargets = (caster.Team == BattleTeam.Player ? teamB : teamA).Where(x => x.IsAlive).ToList();
                    break;
                case AbilityTargetType.SingleAlly:
                    possibleTargets = (caster.Team == BattleTeam.Player ? teamA : teamB).Where(x => x.IsAlive).ToList();
                    break;
                case AbilityTargetType.Self:
                    return caster;
            }

            if (possibleTargets.Count > 0)
                return possibleTargets[Random.Range(0, possibleTargets.Count)];

            return null;
        }

        private void ApplyAbility(BattleCharacter caster, BattleCharacter target, AbilityData ability)
        {
            if (target == null || !target.IsAlive) return;

            int totalDamage = ability.baseDamage + caster.CurrentStats.GetBonusDamage(ability.damageType);
            target.ApplyStatusEffect(ability.effects);
            target.TakeDamage(totalDamage);
            Debug.Log($"{caster.characterName} used {ability.abilityName} on {target.characterName}, dealing {totalDamage} damage!");

            ShowDamagePopup(target, totalDamage);
        }

        private void ShowDamagePopup(BattleCharacter target, int damage)
        {
            if (damagePopupPrefab == null) return;

            var popup = Instantiate(damagePopupPrefab, target.transform.position + Vector3.up + (Vector3.right * Random.Range(-1f, 1f)), Quaternion.identity);
            popup.Setup(damage);
        }
        private IEnumerator BattleLoop()
        {
            while (!IsBattleOver())
            {
                var allCharacters = GetAllCharacters().Where(c => c.IsAlive).ToList();

                foreach (var character in allCharacters)
                {
                    //character.StatusEffectTick(actionInterval);

                    AbilityData ability = character.GetNextReadyAbility();
                    if (ability == null) continue;

                    BattleCharacter target = SelectTarget(character, ability.TargetType);
                    if (target == null) continue;

                    ApplyAbility(character, target, ability);

                    character.PlayAttackAnimation(ability.animationTrigger);
                    character.StartCooldown(ability);

                }
                yield return new WaitForSeconds(actionInterval);

            }

            Debug.Log("Battle Over!");
        }


        private void Update()
        {
            UpdateAllCooldowns(Time.deltaTime);

        }

        private void UpdateAllCooldowns(float deltaTime)
        {
            foreach (var character in teamA.Concat(teamB))
            {
                character.UpdateCooldowns(deltaTime);
            }

            // Обновляем UI с агрегированными кулдаунами для каждой команды
            if (playerAbilityUI != null)
            {
                var aggregatedPlayerCooldowns = AggregateCooldowns(teamA);
                playerAbilityUI.UpdateCooldowns(aggregatedPlayerCooldowns);
            }

            if (enemyAbilityUI != null)
            {
                var aggregatedEnemyCooldowns = AggregateCooldowns(teamB);
                enemyAbilityUI.UpdateCooldowns(aggregatedEnemyCooldowns);
            }
        }

        private Dictionary<AbilityData, float> AggregateCooldowns(List<BattleCharacter> characters)
        {
            Dictionary<AbilityData, float> aggregated = new();

            foreach (var character in characters)
            {
                foreach (var ability in character.Abilities)
                {
                    float cd = character.GetRemainingCooldown(ability);
                    if (aggregated.TryGetValue(ability, out float existingCd))
                    {
                        aggregated[ability] = Mathf.Max(existingCd, cd);
                    }
                    else
                    {
                        aggregated[ability] = cd;
                    }
                }
            }
            return aggregated;
        }
    }

}
