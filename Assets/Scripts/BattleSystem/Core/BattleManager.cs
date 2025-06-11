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

        private bool battleStarted = false;
        [Header("UI")]
        public AbilityBarUI playerAbilityUI;
        public AbilityBarUI enemyAbilityUI;
        public DamagePopup damagePopupPrefab;
        private void Start()
        {
            teamA = FindObjectsOfType<BattleCharacter>().Where(c => c.Team == BattleTeam.Player).ToList();
            teamB = FindObjectsOfType<BattleCharacter>().Where(c => c.Team == BattleTeam.Enemy).ToList();
            if (teamA.Count > 0)
                playerAbilityUI.Setup(teamA[0].Abilities);

            if (teamB.Count > 0)
                enemyAbilityUI.Setup(teamB[0].Abilities);
            StartCoroutine(BattleLoop());
        }

        private IEnumerator BattleLoop()
        {
            battleStarted = true;
            Debug.Log("Battle started!");

            while (battleStarted)
            {
                var allCharacters = GetAllCharacters().Where(c => c.IsAlive).ToList();

                foreach (var character in allCharacters)
                {
                    if (!character.IsAlive) continue;

                    AbilityData ability = character.GetNextAbility();
                    if (ability == null) continue;

                    BattleCharacter target = SelectTarget(character, ability.TargetType);
                    yield return new WaitForSeconds(ability.castTime); // simulate cast

                    ApplyAbility(character, target, ability);

                    yield return new WaitForSeconds(ability.cooldown); // wait for next action
                }

                if (teamA.All(c => !c.IsAlive) || teamB.All(c => !c.IsAlive))
                {
                    Debug.Log("Battle over!");
                    battleStarted = false;
                }

                yield return null;
            }
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
            Debug.Log($"{caster.characterName} used {ability.abilityName} on {target.characterName}, dealing {totalDamage} damage!");

            target.CurrentStats.CurrentHP = Mathf.Max(0, target.CurrentStats.CurrentHP);

            if (target.CurrentStats.CurrentHP <= 0)
            {
                Debug.Log($"{target.characterName} has been defeated!");
            }

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

