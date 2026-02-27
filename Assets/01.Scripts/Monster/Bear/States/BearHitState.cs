using UnityEngine;

public class BearHitState : BearStateBase
{
    public override int StateId => (int)EBearState.Hit;

    private const float HIT_RECOVERY_TIME = 0.8f;
    private float _hitRecoveryTimer;

    public BearHitState(BearController ctx) : base(ctx) { }

    public override void Enter()
    {
        _ctx.Agent.isStopped = true;
        _hitRecoveryTimer    = HIT_RECOVERY_TIME;
        _ctx.TriggerHitAnim();
    }

    public override void Update()
    {
        _hitRecoveryTimer -= Time.deltaTime;
        if (_hitRecoveryTimer <= 0f)
            _ctx.ChangeState(new BearApproachState(_ctx));
    }
}
