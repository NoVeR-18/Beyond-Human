using UnityEngine;

namespace BattleSystem
{
    [CreateAssetMenu(menuName = "Battle/Status Effect")]
    [System.Serializable]
    public class StatusEffect : ScriptableObject
    {
        public string effectName;
        public StatusType Type;
        public float Duration;
        public int damagePerTick;
    }

}
