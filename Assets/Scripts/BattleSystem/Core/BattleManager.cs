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
        private float timer = 0f;

        [Header("UI")]
        public AbilityBarUI playerAbilityUI;
        public AbilityBarUI enemyAbilityUI;
        public DamagePopup damagePopupPrefab;

        private void Start()
        {
            teamA = FindObjectsOfType<BattleCharacter>().Where(c => c.Team == BattleTeam.Player).ToList();
            teamB = FindObjectsOfType<BattleCharacter>().Where(c => c.Team == BattleTeam.Enemy).ToList();

            if (teamA.Count > 0) playerAbilityUI.Setup(teamA[0].Abilities);
            if (teamB.Count > 0) enemyAbilityUI.Setup(teamB[0].Abilities);
        }

        private void Update()
        {
            if (IsBattleOver()) return;

            timer += Time.deltaTime;
            playerAbilityUI?.UpdateCooldowns(teamA[0]);
            enemyAbilityUI?.UpdateCooldowns(teamB[0]);
            if (timer >= actionInterval)
            {
                timer = 0f;
                ProcessTurn();
            }
        }

        private void ProcessTurn()
        {
            var allCharacters = GetAllCharacters().Where(c => c.IsAlive).ToList();

            foreach (var character in allCharacters)
            {
                character.TickCooldowns(actionInterval);

                AbilityData ability = character.GetNextReadyAbility();
                if (ability == null) continue;

                BattleCharacter target = SelectTarget(character, ability.TargetType);
                if (target == null) continue;

                ApplyAbility(character, target, ability);
                character.StartCooldown(ability);
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
            target.CurrentStats.CurrentHP -= totalDamage;
            target.CurrentStats.CurrentHP = Mathf.Max(0, target.CurrentStats.CurrentHP);

            Debug.Log($"{caster.characterName} used {ability.abilityName} on {target.characterName}, dealing {totalDamage} damage!");

            if (target.CurrentStats.CurrentHP <= 0)
                Debug.Log($"{target.characterName} has been defeated!");
            ShowDamagePopup(target, totalDamage);
        }

        private void ShowDamagePopup(BattleCharacter target, int damage)
        {
            if (damagePopupPrefab == null) return;

            var popup = Instantiate(damagePopupPrefab, target.transform.position + Vector3.up, Quaternion.identity);
            popup.Setup(damage);
        }
    }
}
