using System;
using UnityEngine;

[Serializable]
public class BearStat
{
    // Inspector에서 설정: maxValue=300, regenRate=0
    public ConsumableStat Health;

    // Inspector에서 설정: Damage=20, MoveSpeed=3.5, RunSpeed=6
    public ValueStat Damage;
    public ValueStat MoveSpeed;
    public ValueStat RunSpeed;

    [SerializeField] private float _detectRange    = 10f;
    [SerializeField] private float _attackRange    = 2.5f;
    [SerializeField] private float _returnRange    = 20f;
    [SerializeField] private float _attackCooldown = 2f;
    [SerializeField] private float _patrolRadius   = 8f;

    public float DetectRange    => _detectRange;
    public float AttackRange    => _attackRange;
    public float ReturnRange    => _returnRange;
    public float AttackCooldown => _attackCooldown;
    public float PatrolRadius   => _patrolRadius;
}
