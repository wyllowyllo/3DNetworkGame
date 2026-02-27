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
    protected BearController _ctx;

    public abstract int StateId { get; }

    protected BearStateBase(BearController ctx)
    {
        _ctx = ctx;
    }

    public abstract void Enter();
    public abstract void Update();
    public virtual  void Exit() { }

    public virtual void OnTakeDamage(float damage, int attackerActorNumber)
    {
        _ctx.Stat.Health.Consume(damage);

        if (_ctx.Stat.Health.Value <= 0f)
            _ctx.ChangeState(new BearDeathState(_ctx));
        else
            _ctx.ChangeState(new BearHitState(_ctx));
    }

    protected void DetectPlayer()
    {
        Collider[] hits = Physics.OverlapSphere(
            _ctx.transform.position, _ctx.Stat.DetectRange,
            LayerMask.GetMask("Player"));

        _ctx.Target = null;
        float minDist = float.MaxValue;

        foreach (var col in hits)
        {
            float d = Vector3.Distance(_ctx.transform.position, col.transform.position);
            if (d < minDist)
            {
                minDist     = d;
                _ctx.Target = col.transform;
            }
        }
    }

    protected Vector3 GetRandomPatrolPoint()
    {
        for (int i = 0; i < 10; i++)
        {
            Vector3 randomDir = Random.insideUnitSphere * _ctx.Stat.PatrolRadius;
            randomDir += _ctx.BasePosition;

            if (NavMesh.SamplePosition(randomDir, out NavMeshHit hit, _ctx.Stat.PatrolRadius, NavMesh.AllAreas))
                return hit.position;
        }

        return _ctx.BasePosition;
    }
}
