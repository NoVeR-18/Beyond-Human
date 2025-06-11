using UnityEngine;

namespace BattleSystem
{
    [CreateAssetMenu(menuName = "Battle/Status Effect")]
    public class StatusEffectData : ScriptableObject
    {
        public string effectName;
        public StatusType type; // Affliction, Buff, Debuff, Control
        public float duration;
        public int potency;
    }

}
