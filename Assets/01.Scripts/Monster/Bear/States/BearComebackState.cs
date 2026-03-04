public class BearComebackState : BearStateBase
{
    public override int StateId => (int)EBearState.Comeback;

    public BearComebackState(BearController bearController) : base(bearController) { }

    public override void Enter()
    {
        BearController.nav.isStopped = false;
        BearController.nav.speed     = BearController.Stat.MoveSpeed.Value;
        BearController.nav.SetDestination(BearController.BasePosition);
    }

    public override void Update()
    {
        if (BearController.Target != null)
        {
            BearController.ChangeState(new BearApproachState(BearController));
            return;
        }

        if (!BearController.nav.pathPending && BearController.nav.remainingDistance < 1f)
            BearController.ChangeState(new BearIdleState(BearController));
    }
}
