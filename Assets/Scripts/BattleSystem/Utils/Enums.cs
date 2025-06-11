namespace BattleSystem
{
    public enum StatusType
    {
        Affliction, //  DoT
        Control,    //  ����, ������� � �.�.
        Debuff,     //  ����������
        Buff,       //  ��������
        Elemental   //  ��������� � �.�.
    }

    public enum BattleTeam
    {
        Player,
        Enemy
    }
    public enum AbilityTargetType
    {
        SingleEnemy,
        SingleAlly,
        AllEnemies,
        AllAllies,
        Self,
        All // �������� ����
    }
    public enum DamageType
    {
        None,
        Fire,
        Water,
        Ice,
        Electric,
        Poison,
        Blunt,
        Slash,
        Pierce,
        Holy,
        Dark,
        Earth,
        Air
    }
    public enum AbilityType
    {
        Physical,
        Magical,
        Neutral
    }

}