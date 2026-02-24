
using System;

[Serializable]
public class PlayerStat
{
    // 기본 스탯
    public ConsumableStat Health;    // MaxValue: 100, RegenRate: 0
    public ConsumableStat Stamina;   // MaxValue: 100, RegenRate: 100

    // 이동 스탯
    public ValueStat MoveSpeed;      // 7
    public ValueStat RunSpeed;       // 10
    public ValueStat JumpPower;      // 2.5
    public ValueStat RotationSpeed;  // 100
    
    // 공격 스탯
    public ValueStat AttackSpeed;    // 0.6
    public ValueStat Damage;
}
