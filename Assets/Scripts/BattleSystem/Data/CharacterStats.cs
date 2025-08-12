namespace BattleSystem
{

    [System.Serializable]
    public class CharacterStats
    {
        public int MaxHP = 100;
        public int CurrentHP = 100;

        public int CurrentShield = 0;
        public int airResistance = 0;
        public int waterResistance = 0;
        public int fireResistance = 0;
        public int earthResistance = 0;
        public int electricResistance = 0;
        public int iceResistance = 0;
        public int poisonResistance = 0;
        public int bluntResistance = 0;
        public int piercingResistance = 0;
        public int curseResistance = 0;
        public int holyResistance = 0;

        public int airDamage = 0;
        public int waterDamage = 0;
        public int fireDamage = 0;
        public int earthDamage = 0;
        public int electricDamage = 0;
        public int iceDamage = 0;
        public int poisonDamage = 0;
        public int bluntDamage = 0;
        public int piercingDamage = 0;
        public int curseDamage = 0;
        public int holyDamage = 0;
    }
}