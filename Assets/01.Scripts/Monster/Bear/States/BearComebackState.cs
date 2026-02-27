public class BearComebackState : BearStateBase
{
    public override int StateId => (int)EBearState.Comeback;

    public BearComebackState(BearController ctx) : base(ctx) { }

    public override void Enter()
    {
        _ctx.Target          = null;
        _ctx.Agent.isStopped = false;
        _ctx.Agent.speed     = _ctx.Stat.MoveSpeed.Value;
        _ctx.Agent.SetDestination(_ctx.BasePosition);
    }

    public override void Update()
    {
        DetectPlayer();
        if (_ctx.Target != null)
        {
            _ctx.ChangeState(new BearApproachState(_ctx));
            return;
        }

        if (!_ctx.Agent.pathPending && _ctx.Agent.remainingDistance < 1f)
            _ctx.ChangeState(new BearIdleState(_ctx));
    }
}
