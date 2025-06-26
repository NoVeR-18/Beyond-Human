using System.Collections.Generic;
using UnityEngine;

namespace BattleSystem
{
    public class AbilityBarUI : MonoBehaviour
    {
        [SerializeField] private Transform abilityContainer;
        [SerializeField] private GameObject abilitySlotPrefab;

        private Dictionary<AbilitySlotUI, BattleCharacter> slots = new();

        private void Awake()
        {
            foreach (Transform child in abilityContainer)
                Destroy(child.gameObject);

            slots.Clear();
        }

        public void Setup(List<AbilityData> abilities, BattleCharacter owner)
        {
            foreach (var ability in abilities)
            {
                var go = Instantiate(abilitySlotPrefab, abilityContainer);
                var slot = go.GetComponent<AbilitySlotUI>();
                slot.Init(ability);
                slots.Add(slot, owner);
            }
        }

        public void UpdateCooldowns(Dictionary<AbilityData, float> cooldowns, BattleCharacter owner)
        {
            List<AbilitySlotUI> toRemove = new();

            foreach (var pair in slots)
            {
                var slot = pair.Key;
                var character = pair.Value;

                if (character != owner)
                    continue;

                var ability = slot.GetAbility();

                if (cooldowns.TryGetValue(ability, out float remaining))
                {
                    slot.UpdateCooldown(remaining, ability.cooldown);
                }
                else
                {
                    slot.UpdateCooldown(0, ability.cooldown);
                }
            }
        }

        public void RemoveDissabledAbilities(List<AbilityData> toRemove, BattleCharacter owner)
        {
            var toRemoveSlots = new List<AbilitySlotUI>();

            foreach (var pair in slots)
            {
                var slot = pair.Key;
                var character = pair.Value;

                if (character != owner)
                    continue;

                var ability = slot.GetAbility();
                if (toRemove.Contains(ability))
                {
                    toRemoveSlots.Add(slot);
                }
            }

            foreach (var slot in toRemoveSlots)
            {
                slots.Remove(slot);
                Destroy(slot.gameObject);
            }
        }
    }
}
