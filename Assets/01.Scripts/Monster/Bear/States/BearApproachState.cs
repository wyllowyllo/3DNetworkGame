using UnityEngine;

public class BearApproachState : BearStateBase
{
    public override int StateId => (int)EBearState.Approach;

    public BearApproachState(BearController ctx) : base(ctx) { }

    public override void Enter()
    {
        _ctx.Agent.isStopped = false;
        _ctx.Agent.speed     = _ctx.Stat.RunSpeed.Value;
    }

    public override void Update()
    {
        if (_ctx.Target == null)
        {
            _ctx.ChangeState(new BearComebackState(_ctx));
            return;
        }

        if (Vector3.Distance(_ctx.transform.position, _ctx.BasePosition) > _ctx.Stat.ReturnRange)
        {
            _ctx.ChangeState(new BearComebackState(_ctx));
            return;
        }

        _ctx.Agent.SetDestination(_ctx.Target.position);

        if (Vector3.Distance(_ctx.transform.position, _ctx.Target.position) <= _ctx.Stat.AttackRange)
            _ctx.ChangeState(new BearAttackState(_ctx));
    }
}
