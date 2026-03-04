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
/// Bear 상태 공통 추상 기반.
/// BearController 참조와 GetRandomPatrolPoint 유틸을 제공한다.
/// 플레이어 탐지는 BearController.DetectTarget() / .ClearTarget()을 사용한다.
/// 데미지/상태 전환 결정은 BearController.TakeDamage가 전담하므로 OnTakeDamage는 없다.
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
