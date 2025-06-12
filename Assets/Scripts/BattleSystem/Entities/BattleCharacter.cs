using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace BattleSystem
{

    public class BattleCharacter : MonoBehaviour
    {
        public string characterName;
        public BattleTeam Team;
        public CharacterStats CurrentStats;

        public List<AbilityData> Abilities = new();

        private Dictionary<AbilityData, float> cooldowns = new();

        public bool IsAlive => CurrentStats.CurrentHP > 0;
        private void Start()
        {
            cooldowns = new Dictionary<AbilityData, float>();

            foreach (var ability in Abilities)
            {
                if (!cooldowns.ContainsKey(ability))
                    cooldowns[ability] = 0f;
            }
        }
        public void StartCooldown(AbilityData ability)
        {
            if (ability == null) return;
            cooldowns[ability] = ability.cooldown;
        }
        public void TickCooldowns(float deltaTime)
        {
            var keys = cooldowns.Keys.ToList();
            foreach (var key in keys)
            {
                cooldowns[key] -= deltaTime;
                if (cooldowns[key] <= 0)
                {
                    cooldowns[key] = 0;
                }
            }
        }
        public AbilityData GetNextReadyAbility()
        {
            foreach (var ability in Abilities)
            {
                if (cooldowns.TryGetValue(ability, out float remaining) && remaining <= 0)
                    return ability;
            }
            return null;
        }
        public float GetRemainingCooldown(AbilityData ability)
        {
            if (!cooldowns.ContainsKey(ability)) return 0;
            return cooldowns[ability];
        }
    }


}