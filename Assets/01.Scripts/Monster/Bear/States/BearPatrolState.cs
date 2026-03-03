using UnityEngine;

public class BearPatrolState : BearStateBase
{
    public override int StateId => (int)EBearState.Patrol;

    public BearPatrolState(BearController bearController) : base(bearController) { }

    public override void Enter()
    {
        BearController.Agent.isStopped = false;
        BearController.Agent.speed     = BearController.Stat.MoveSpeed.Value;
        BearController.Agent.SetDestination(GetRandomPatrolPoint());
    }

    public override void Update()
    {
        DetectPlayer();
        if (BearController.Target != null)
        {
            BearController.ChangeState(new BearApproachState(BearController));
            return;
        }

        if (!BearController.Agent.pathPending && BearController.Agent.remainingDistance < 1f)
            BearController.ChangeState(new BearIdleState(BearController));
    }
}
