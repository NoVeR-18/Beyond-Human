using System.Collections.Generic;
using UnityEngine;

namespace BattleSystem
{
    public class AbilityBarUI : MonoBehaviour
    {
        [SerializeField] private Transform abilityContainer;
        [SerializeField] private GameObject abilitySlotPrefab;


        private List<AbilitySlotUI> slots = new();

        public void Setup(List<AbilityData> abilities)
        {
            foreach (Transform child in abilityContainer)
                Destroy(child.gameObject);

            slots.Clear();

            foreach (var ability in abilities)
            {
                var go = Instantiate(abilitySlotPrefab, abilityContainer);
                var slot = go.GetComponent<AbilitySlotUI>();
                slot.Init(ability);
                slots.Add(slot);
            }
        }

        public void UpdateCooldowns(BattleCharacter character)
        {
            foreach (var slot in slots)
            {
                var ability = slot.GetAbility();
                float max = ability.cooldown;
                float remaining = character.GetRemainingCooldown(ability);
                slot.UpdateCooldown(remaining, max);
            }
        }
    }


}