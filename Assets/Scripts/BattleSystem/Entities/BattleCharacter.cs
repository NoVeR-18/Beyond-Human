using System.Collections.Generic;
using UnityEngine;
namespace BattleSystem
{

    public class BattleCharacter : MonoBehaviour
    {
        public string characterName;
        public BattleTeam Team;
        public CharacterStats CurrentStats;

        public List<AbilityData> Abilities = new();
        private int currentAbilityIndex = 0;

        public bool IsAlive => CurrentStats.CurrentHP > 0;

        public AbilityData GetNextAbility()
        {
            if (Abilities.Count == 0) return null;
            var ability = Abilities[currentAbilityIndex];
            currentAbilityIndex = (currentAbilityIndex + 1) % Abilities.Count;
            return ability;
        }
    }


}