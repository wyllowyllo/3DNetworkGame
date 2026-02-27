using UnityEngine;

public class BearAttackWaitState : BearStateBase
{
    public override int StateId => (int)EBearState.AttackWait;

    private float _attackWaitTimer;

    public BearAttackWaitState(BearController ctx) : base(ctx) { }

    public override void Enter()
    {
        _ctx.Agent.isStopped = true;
        _attackWaitTimer     = _ctx.Stat.AttackCooldown;
    }

    public override void Update()
    {
        _attackWaitTimer -= Time.deltaTime;
        if (_attackWaitTimer > 0f) return;

        if (_ctx.Target == null)
        {
            _ctx.ChangeState(new BearIdleState(_ctx));
            return;
        }

        float dist = Vector3.Distance(_ctx.transform.position, _ctx.Target.position);
        if (dist <= _ctx.Stat.AttackRange)
            _ctx.ChangeState(new BearAttackState(_ctx));
        else
            _ctx.ChangeState(new BearApproachState(_ctx));
    }
}
