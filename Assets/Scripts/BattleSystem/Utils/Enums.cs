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
        Team1,
        Team2,
        Team3
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
    public enum EffectTrigger
    {
        None,
        OnApply,
        OnTick,
        OnRemove,
        OnHitByAbility
    }
}