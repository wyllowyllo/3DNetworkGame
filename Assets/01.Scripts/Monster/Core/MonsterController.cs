using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// 몬스터 FSM 의 추상 기반 클래스.
/// Agent/Animator 캐시, BasePosition, ChangeState를 제공한다.
/// MasterClient일 때만 CurrentState.Update()를 호출한다.
/// Target 및 데미지 처리는 각 Controller(BearController)가 전담한다.
/// </summary>
public abstract class MonsterController : MonoBehaviourPunCallbacks
{
    [SerializeField] protected IMonsterState _currentState;
    
    public NavMeshAgent nav    { get; private set; }
    public Animator     Animator { get; private set; }

    public Vector3 BasePosition { get; protected set; }

    
    public int CurrentStateId => CurrentState?.StateId ?? 0;

    protected IMonsterState CurrentState
    {
        get => _currentState;
        set => _currentState = value;
    }

    protected virtual void Awake()
    {
        nav    = GetComponent<NavMeshAgent>();
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
}
