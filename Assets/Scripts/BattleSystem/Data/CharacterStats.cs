namespace BattleSystem
{

    [System.Serializable]
    public class CharacterStats
    {
        public int MaxHP = 100;
        public int CurrentHP = 100;

        public int FireDamage = 0;
        public int WaterDamage = 0;
        public int BluntDamage = 0;

        public int GetBonusDamage(DamageType type)
        {
            return type switch
            {
                DamageType.Fire => FireDamage,
                DamageType.Water => WaterDamage,
                DamageType.Blunt => BluntDamage,
                _ => 0,
            };
        }
    }


}