using UnityEngine;

public class BearPatrolState : BearStateBase
{
    public override int StateId => (int)EBearState.Patrol;

    public BearPatrolState(BearController ctx) : base(ctx) { }

    public override void Enter()
    {
        _ctx.Agent.isStopped = false;
        _ctx.Agent.speed     = _ctx.Stat.MoveSpeed.Value;
        _ctx.Agent.SetDestination(GetRandomPatrolPoint());
    }

    public override void Update()
    {
        DetectPlayer();
        if (_ctx.Target != null)
        {
            _ctx.ChangeState(new BearMoveAttackState(_ctx));
            return;
        }

        if (Vector3.Distance(_ctx.transform.position, _ctx.BasePosition) > _ctx.Stat.ReturnRange)
        {
            _ctx.ChangeState(new BearMoveReturnState(_ctx));
            return;
        }

        if (!_ctx.Agent.pathPending && _ctx.Agent.remainingDistance < 1f)
            _ctx.ChangeState(new BearIdleState(_ctx));
    }
}
