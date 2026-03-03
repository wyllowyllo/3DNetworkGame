using UnityEngine;

public class BearApproachState : BearStateBase
{
    public override int StateId => (int)EBearState.Approach;

    public BearApproachState(BearController bearController) : base(bearController) { }

    public override void Enter()
    {
        BearController.Agent.isStopped = false;
        BearController.Agent.speed     = BearController.Stat.RunSpeed.Value;
    }

    public override void Update()
    {
        if (BearController.Target == null)
        {
            BearController.ChangeState(new BearComebackState(BearController));
            return;
        }

        if (Vector3.Distance(BearController.transform.position, BearController.BasePosition) > BearController.Stat.ReturnRange)
        {
            BearController.ChangeState(new BearComebackState(BearController));
            return;
        }

        BearController.Agent.SetDestination(BearController.Target.position);

        if (Vector3.Distance(BearController.transform.position, BearController.Target.position) <= BearController.Stat.AttackRange)
            BearController.ChangeState(new BearAttackState(BearController));
    }
}
