using UnityEngine;
using UnityEngine.AI;

public enum EBearState
{
    Idle       = 0,
    Patrol     = 1,
    Comeback   = 2,
    Approach   = 3,
    Attack     = 4,
    AttackWait = 5,
    Hit        = 6,
    Death      = 7
}

/// <summary>
/// Bear 상태 공통 추상 기반. BearController 참조 및 공유 유틸(DetectPlayer, GetRandomPatrolPoint)을 제공한다.
/// 기본 OnTakeDamage: HP 소모 → Death 또는 Hit 전환.
/// </summary>
public abstract class BearStateBase : IMonsterState
{
    protected BearController BearController;

    public abstract int StateId { get; }

    protected BearStateBase(BearController bearController)
    {
        BearController = bearController;
    }

    public abstract void Enter();
    public abstract void Update();
    public virtual  void Exit() { }

    public virtual void OnTakeDamage(float damage, int attackerActorNumber)
    {
        BearController.Stat.Health.Consume(damage);

        if (BearController.Stat.Health.Value <= 0f)
            BearController.ChangeState(new BearDeathState(BearController));
        else
            BearController.ChangeState(new BearHitState(BearController));
    }

    protected void DetectPlayer()
    {
        Collider[] hits = Physics.OverlapSphere(
            BearController.transform.position, BearController.Stat.DetectRange,
            LayerMask.GetMask("Player"));

        BearController.Target = null;
        float minDist = float.MaxValue;

        foreach (var col in hits)
        {
            float d = Vector3.Distance(BearController.transform.position, col.transform.position);
            if (d < minDist)
            {
                minDist     = d;
                BearController.Target = col.transform;
            }
        }
    }

    protected Vector3 GetRandomPatrolPoint()
    {
        for (int i = 0; i < 10; i++)
        {
            Vector3 randomDir = Random.insideUnitSphere * BearController.Stat.PatrolRadius;
            randomDir += BearController.BasePosition;

            if (NavMesh.SamplePosition(randomDir, out NavMeshHit hit, BearController.Stat.PatrolRadius, NavMesh.AllAreas))
                return hit.position;
        }

        return BearController.BasePosition;
    }
}
