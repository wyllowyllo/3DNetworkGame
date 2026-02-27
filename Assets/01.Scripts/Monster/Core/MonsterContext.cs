using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// 몬스터 FSM Context의 추상 기반 클래스.
/// Agent/Animator 캐시, BasePosition/Target, ChangeState, RouteToState를 제공한다.
/// MasterClient일 때만 CurrentState.Update()를 호출한다.
/// </summary>
public abstract class MonsterContext : MonoBehaviourPunCallbacks
{
    public NavMeshAgent Agent    { get; private set; }
    public Animator     Animator { get; private set; }

    public Vector3   BasePosition { get; protected set; }
    public Transform Target       { get; set; }

    protected IMonsterState CurrentState  { get; private set; }
    public    int           CurrentStateId => CurrentState?.StateId ?? 0;

    protected virtual void Awake()
    {
        Agent    = GetComponent<NavMeshAgent>();
        Animator = GetComponent<Animator>();
    }

    protected virtual void Update()
    {
        if (PhotonNetwork.IsMasterClient)
            CurrentState?.Update();
    }

    public void ChangeState(IMonsterState next)
    {
        CurrentState?.Exit();
        CurrentState = next;
        CurrentState?.Enter();
    }

    protected void RouteToState(float dmg, int actor)
    {
        CurrentState?.OnTakeDamage(dmg, actor);
    }
}
