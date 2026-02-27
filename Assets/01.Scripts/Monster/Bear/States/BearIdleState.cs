using UnityEngine;

public class BearIdleState : BearStateBase
{
    public override int StateId => (int)EBearState.Idle;

    private float _idleTimer;

    public BearIdleState(BearController ctx) : base(ctx) { }

    public override void Enter()
    {
        _ctx.Agent.isStopped = true;
        _ctx.Agent.ResetPath();
        _idleTimer = Random.Range(2f, 5f);
    }

    public override void Update()
    {
        _idleTimer -= Time.deltaTime;

        DetectPlayer();
        if (_ctx.Target != null)
        {
            _ctx.ChangeState(new BearApproachState(_ctx));
            return;
        }

        if (_idleTimer <= 0f)
            _ctx.ChangeState(new BearPatrolState(_ctx));
    }
}
