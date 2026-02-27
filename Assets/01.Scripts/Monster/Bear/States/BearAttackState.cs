using UnityEngine;

public class BearAttackState : BearStateBase
{
    public override int StateId => (int)EBearState.Attack;

    private static readonly int[] AttackIndices = { 1, 2, 3, 5 };
    private float _attackCooldownTimer;

    public BearAttackState(BearController ctx) : base(ctx) { }

    public override void Enter()
    {
        _ctx.Agent.isStopped = true;
        _attackCooldownTimer = _ctx.Stat.AttackCooldown;

        _ctx.ApplyDamageToTarget();

        int index = AttackIndices[Random.Range(0, AttackIndices.Length)];
        _ctx.TriggerAttackAnim(index);
    }

    public override void Update()
    {
        _attackCooldownTimer -= Time.deltaTime;
        if (_attackCooldownTimer <= 0f)
            _ctx.ChangeState(new BearAttackWaitState(_ctx));
    }
}
