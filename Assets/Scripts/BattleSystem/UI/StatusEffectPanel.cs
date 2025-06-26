using System.Collections.Generic;
using UnityEngine;

namespace BattleSystem
{

    public class StatusEffectPanel : MonoBehaviour
    {
        [SerializeField] private GameObject statusIconPrefab;
        [SerializeField] private Transform iconContainer;

        private Dictionary<StatusEffectUI, BattleCharacter> effects = new();

        private void Awake()
        {
            foreach (Transform child in iconContainer)
                Destroy(child.gameObject);

            effects.Clear();
        }

        public void Setup(List<StatusEffect> statuses, BattleCharacter owner)
        {
            foreach (var status in statuses)
            {
                var go = Instantiate(statusIconPrefab, iconContainer);
                var slot = go.GetComponent<StatusEffectUI>();
                slot.Init(status);

                effects.Add(slot, owner);
            }
        }

        public void UpdateStatusEffects(Dictionary<StatusEffect, float> statusCooldowns, BattleCharacter owner)
        {
            var toRemove = new List<StatusEffectUI>();

            foreach (var pair in effects)
            {
                var slot = pair.Key;
                var slotOwner = pair.Value;

                if (slotOwner != owner)
                    continue;

                var effect = slot.GetEffect();

                if (statusCooldowns.TryGetValue(effect, out float remaining))
                {
                    if (remaining > 0f)
                    {
                        slot.UpdateCooldown(remaining);
                        continue;
                    }
                }

                // если эффект истёк
                slot.UpdateCooldown(0);
                toRemove.Add(slot);
            }

            foreach (var slot in toRemove)
            {
                effects.Remove(slot);
                Destroy(slot.gameObject);
            }
        }

        public void RemoveDissabledEffects(List<StatusEffect> toRemove, BattleCharacter owner)
        {
            var toRemoveSlots = new List<StatusEffectUI>();

            foreach (var pair in effects)
            {
                var slot = pair.Key;
                var slotOwner = pair.Value;

                if (slotOwner != owner)
                    continue;

                var effect = slot.GetEffect();

                if (toRemove.Contains(effect))
                {
                    toRemoveSlots.Add(slot);
                }
            }

            foreach (var slot in toRemoveSlots)
            {
                effects.Remove(slot);
                Destroy(slot.gameObject);
            }
        }
    }

}


