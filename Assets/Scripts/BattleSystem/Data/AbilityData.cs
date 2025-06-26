using System.Collections.Generic;
using UnityEngine;

namespace BattleSystem
{

    [CreateAssetMenu(fileName = "New Ability", menuName = "Battle/Ability")]
    public class AbilityData : ScriptableObject
    {
        public string abilityName;
        public Sprite icon;
        public float castTime;
        public float cooldown;
        public int baseDamage;
        public DamageType damageType;
        public AbilityTargetType TargetType;
        public AbilityType abilityType;
        public List<StatusEffect> effects; // статус + длительность и т.д.

        public GameObject summonPrefab;
        public string animationTrigger = "Attack";


    }


}