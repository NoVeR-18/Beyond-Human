using System;
using System.Collections.Generic;
using UnityEngine;

namespace BattleSystem
{
    [CreateAssetMenu(fileName = "New Ability", menuName = "Battle/Ability")]
    public class AbilityData : ScriptableObject
    {
        public string abilityName;
        public int tier;
        public Sprite icon;
        public float castTime;
        public float cooldown;
        public int shieldAmount;
        public List<DamageClass> damages;
        public AbilityTargetType TargetType;
        public AbilityType abilityType;
        public List<StatusEffect> effects; // статус + длительность и т.д.

        public GameObject summonPrefab;
        public string animationTrigger = "Attack";
        [Header("Effects")]
        public EffectController effectPrefab;
        public bool attachToTarget = true; // флаг Ч эффект "врезаетс€" в цель или летит к ней
        public float effectMoveSpeed = 10f;

    }

    [Serializable]
    public class DamageClass
    {
        public DamageType damageType;
        public int baseDamage;
    }

}