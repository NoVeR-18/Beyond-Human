using System.Collections.Generic;
using UnityEngine;

namespace BattleSystem
{
    [System.Serializable]
    public class StatusCombo
    {
        public StatusEffect otherEffect;
        public StatusEffect resultingEffect;
        public bool removeOriginals;
    }

    [CreateAssetMenu(menuName = "Battle/Status Effect")]
    public class StatusEffect : ScriptableObject
    {
        public string effectName;
        public Sprite Icon;
        public StatusType Type;
        public float Duration;
        public int damagePerTick;

        public DamageType weaknessToDamageType; // напр. Burned → урон от воды x2
        public float weaknessMultiplier = 1f;
        public bool preventAction;               // Нельзя действовать
        public bool preventMagic;                // Нельзя использовать магию
        public bool preventPhysical;             // Нельзя использовать физику
        public bool silence;                     // Отключить магию
        public bool sleep;                       // Сон
        public bool isFeared;                    // Паника — до первого урона
        public bool forceAllyTargeting;          // Только союзники
        public bool randomTargeting;             // Случайный выбор между врагами и союзниками
        public bool blockNextAbility;            // Следующее умение заблокировано
        public bool doubleMagic;                 // Магия кастуется дважды
        public bool doublePhysical;              // Физика кастуется дважды
        public bool regenHP;                     // Лечение по времени
        public int regenAmount;

        public float castSpeedMultiplier = 1f;   // 0.6f при Chilled
        public float magicDamageModifier = 1f;   // 0.8f при Mind Shackled
        public float physicalDamageModifier = 1f;// 0.8f при Weakened
        public float resistanceModifier = 1f;    // 0.8f при Shattered
        public List<StatusCombo> comboEffects = new(); // комбо-реакции

        public override bool Equals(object obj)
        {
            if (obj is not StatusEffect other) return false;
            return effectName == other.effectName;
        }

        public override int GetHashCode()
        {
            return effectName.GetHashCode();
        }
    }

}
