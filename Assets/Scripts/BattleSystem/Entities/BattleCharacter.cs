using DG.Tweening;
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
        public AbilityBarUI abilityUI;
        private Dictionary<AbilityData, float> cooldowns = new();

        [SerializeField]
        private Animator animator;

        [SerializeField] private GameObject healthBarPrefab;
        private HealthBarUI healthBarInstance;


        public bool IsAlive => CurrentStats.CurrentHP > 0;
        private void Start()
        {
            cooldowns = new Dictionary<AbilityData, float>();

            foreach (var ability in Abilities)
            {
                if (!cooldowns.ContainsKey(ability))
                    cooldowns[ability] = 0f;
            }

            if (healthBarPrefab == null)
            {
                Resources.Load<GameObject>("HealthBar");
            }
            GameObject go = Instantiate(healthBarPrefab, transform);
            go.transform.localPosition = new Vector3(0, 1.5f, 0);
            healthBarInstance = go.GetComponent<HealthBarUI>();
            healthBarInstance.SetHealth(CurrentStats.CurrentHP, CurrentStats.MaxHP);

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
        public void UpdateCooldowns(float deltaTime)
        {
            foreach (var ability in Abilities)
            {
                if (cooldowns.ContainsKey(ability) && cooldowns[ability] > 0f)
                {
                    cooldowns[ability] -= deltaTime;
                    cooldowns[ability] = Mathf.Max(0f, cooldowns[ability]);
                }
            }

        }
        public void TakeDamage(int amount)
        {
            CurrentStats.CurrentHP -= amount;
            CurrentStats.CurrentHP = Mathf.Max(0, CurrentStats.CurrentHP);

            if (healthBarInstance != null)
                healthBarInstance.SetHealth(CurrentStats.CurrentHP, CurrentStats.MaxHP);
            PlayHitReaction();
            if (CurrentStats.CurrentHP <= 0)
            {
                Die();
            }
        }

        private void Die()
        {
            Debug.Log($"{characterName} has been defeated!");
            Destroy(gameObject);
        }
        public void PlayAttackAnimation(string skillTrigger = "Attack")
        {
            if (animator != null && HasParameter(skillTrigger, AnimatorControllerParameterType.Trigger))
            {
                animator.SetTrigger(skillTrigger);
            }
            else
            {
                Debug.LogWarning($"Animator: нет триггера {skillTrigger}, проигрываю обычную анимацию");
                FlashColor(Color.yellow, 0.2f); // или проигрываем базовую атаку
            }
        }
        public void PlayHitReaction()
        {
            if (animator != null)
            {
                if (HasParameter("Hit", AnimatorControllerParameterType.Trigger))
                {
                    animator.SetTrigger("Hit");
                }
                else
                {
                    FlashColor(Color.red, 0.2f);
                }
            }
        }
        public void FlashColor(Color flashColor, float duration)
        {
            var renderers = GetComponentsInChildren<SpriteRenderer>();

            foreach (var r in renderers)
            {
                var originalColor = r.color;

                r.DOColor(flashColor, 0.05f) // Быстро покрасить
                 .OnComplete(() =>
                     r.DOColor(originalColor, duration) // Затем вернуть назад
                 );
            }
        }
        public bool HasParameter(string name, AnimatorControllerParameterType type)
        {
            if (animator == null) return false;

            foreach (var param in animator.parameters)
            {
                if (param.name == name && param.type == type)
                    return true;
            }

            return false;
        }


    }


}