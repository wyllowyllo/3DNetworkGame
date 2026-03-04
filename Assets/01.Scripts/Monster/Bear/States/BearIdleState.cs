using UnityEngine;

public class BearIdleState : BearStateBase
{
    public override int StateId => (int)EBearState.Idle;

    private float _idleTimer;

    public BearIdleState(BearController bearController) : base(bearController) { }

    public override void Enter()
    {
        BearController.nav.isStopped = true;
        BearController.nav.ResetPath();
        _idleTimer = Random.Range(2f, 5f);
    }

    public override void Update()
    {
        _idleTimer -= Time.deltaTime;

        if (BearController.Target != null)
        {
            BearController.ChangeState(new BearApproachState(BearController));
            return;
        }

        if (_idleTimer <= 0f)
            BearController.ChangeState(new BearPatrolState(BearController));
    }
}
