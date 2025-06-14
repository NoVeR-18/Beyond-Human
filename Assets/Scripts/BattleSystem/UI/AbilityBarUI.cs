using System.Collections.Generic;
using UnityEngine;

namespace BattleSystem
{
    public class AbilityBarUI : MonoBehaviour
    {
        [SerializeField] private Transform abilityContainer;
        [SerializeField] private GameObject abilitySlotPrefab;


        private List<AbilitySlotUI> slots = new();
        private void Awake()
        {
            foreach (Transform child in abilityContainer)
                Destroy(child.gameObject);
            slots.Clear();

        }
        public void Setup(List<AbilityData> abilities)
        {

            foreach (var ability in abilities)
            {
                var go = Instantiate(abilitySlotPrefab, abilityContainer);
                var slot = go.GetComponent<AbilitySlotUI>();
                slot.Init(ability);
                slots.Add(slot);
            }
        }

        public void UpdateCooldowns(Dictionary<AbilityData, float> cooldowns)
        {
            foreach (var slot in slots)
            {
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

    }


}