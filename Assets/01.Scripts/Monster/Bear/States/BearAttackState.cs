using UnityEngine;

public class BearAttackState : BearStateBase
{
    public override int StateId => (int)EBearState.Attack;

    private static readonly int[] AttackIndices = { 1, 2, 3, 5 };
    private float _timer;

    public BearAttackState(BearController bearController) : base(bearController) { }

    public override void Enter()
    {
        BearController.Agent.isStopped = true;
        _timer = BearController.Stat.AttackCooldown;
        BearController.SetAttackStance(true);
    }

    public override void Update()
    {
        if (BearController.Target == null)
        {
            BearController.ChangeState(new BearComebackState(BearController));
            return;
        }

        if (Vector3.Distance(BearController.transform.position, BearController.Target.position) > BearController.Stat.AttackRange)
        {
            BearController.ChangeState(new BearApproachState(BearController));
            return;
        }

        _timer -= Time.deltaTime;
        if (_timer <= 0f)
        {
            _timer = BearController.Stat.AttackCooldown;
            BearController.ApplyDamageToTarget();
            int index = AttackIndices[Random.Range(0, AttackIndices.Length)];
            BearController.TriggerAttackAnim(index);
        }
    }

    public override void Exit()
    {
        BearController.SetAttackStance(false);
    }
}
