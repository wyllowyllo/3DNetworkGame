using Photon.Pun;
using UnityEngine;

public class BearHitState : BearStateBase
{
    public override int StateId => (int)EBearState.Hit;

    private const float HIT_RECOVERY_TIME = 0.8f;
    private float _hitRecoveryTimer;

    public BearHitState(BearController bearController) : base(bearController) { }

    public override void Enter()
    {
        BearController.nav.isStopped = true;
        _hitRecoveryTimer    = HIT_RECOVERY_TIME;
        BearController.photonView.RPC(nameof(BearController.PlayHitAnimation), RpcTarget.All);
    }

    public override void Update()
    {
        _hitRecoveryTimer -= Time.deltaTime;
        if (_hitRecoveryTimer <= 0f)
            BearController.ChangeState(new BearApproachState(BearController));
    }
}
