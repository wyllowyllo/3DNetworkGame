using System.Collections;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

/// <summary>
/// BearMonster.cs를 대체하는 Context 클래스.
/// FSM 초기화, 외부 이벤트 수신, 상태 전환 명령 실행만 담당한다.
/// 게임 로직은 각 BearXxxState에 위임한다.
/// </summary>
public class BearController : MonsterContext, IPunObservable, IMonsterController
{
    [SerializeField] private BearStat _stat;
    [SerializeField] private float    _respawnDelay = 8f;

    public BearStat Stat => _stat;

    // 프록시 클라이언트 동기화 값
    private float _syncedHP;
    private int   _syncedState;
    private float _syncedSpeed;

    // ─── IMonsterController ───────────────────────────────────────
    public float CurrentHP => _stat.Health.Value;
    public float MaxHP     => _stat.Health.MaxValue;
    public bool  IsDead    => _stat.Health.Value <= 0f;

    // ─── 초기화 ───────────────────────────────────────────────────

    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        BasePosition = transform.position;

        if (PhotonNetwork.IsMasterClient)
        {
            _stat.Health.Initialize();
            ChangeState(new BearIdleState(this));
        }
    }

    // ─── 업데이트 ─────────────────────────────────────────────────

    protected override void Update()
    {
        base.Update(); // MasterClient이면 CurrentState.Update() 호출

        if (PhotonNetwork.IsMasterClient)
        {
            float speed = (Agent != null && Agent.enabled) ? Agent.velocity.magnitude : 0f;
            Animator.SetFloat("Speed", speed);
        }
        else
        {
            Animator.SetFloat("Speed", _syncedSpeed);
            Animator.SetBool("IsAttacking", _syncedState == (int)EBearState.Attack);
        }
    }

    // ─── IDamagable ───────────────────────────────────────────────

    [PunRPC]
    public void TakeDamage(float damage, int attackerActorNumber)
    {
        if (!PhotonNetwork.IsMasterClient) return;
        if (IsDead) return;

        RouteToState(damage, attackerActorNumber);
    }

    // ─── IPunObservable (30 Hz) ───────────────────────────────────

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(_stat.Health.Value);
            stream.SendNext(CurrentStateId);
            float speed = (Agent != null && Agent.enabled) ? Agent.velocity.magnitude : 0f;
            stream.SendNext(speed);
        }
        else
        {
            _syncedHP    = (float)stream.ReceiveNext();
            _syncedState = (int)stream.ReceiveNext();
            _syncedSpeed = (float)stream.ReceiveNext();

            _stat.Health.SetValue(_syncedHP);
        }
    }

    // ─── MasterClient 전환 ────────────────────────────────────────

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        _stat.Health.SetValue(_syncedHP);

        EBearState restoreState = (EBearState)_syncedState;
        if (restoreState == EBearState.Death)
        {
            _stat.Health.Initialize();
            Agent.enabled = true;
            photonView.RPC(nameof(RPC_Respawn), RpcTarget.All, BasePosition);
            restoreState = EBearState.Idle;
        }

        ChangeState(CreateState(restoreState));
    }

    private IMonsterState CreateState(EBearState state)
    {
        switch (state)
        {
            case EBearState.Idle:       return new BearIdleState(this);
            case EBearState.Patrol:     return new BearPatrolState(this);
            case EBearState.Comeback:   return new BearComebackState(this);
            case EBearState.Approach:   return new BearApproachState(this);
            case EBearState.Attack:     return new BearAttackState(this);
            case EBearState.Hit:        return new BearHitState(this);
            case EBearState.Death:      return new BearDeathState(this);
            default:                    return new BearIdleState(this);
        }
    }

    // ─── RPC 래퍼 메서드 (상태 클래스에서 호출) ───────────────────

    public void SetAttackStance(bool active)    => Animator.SetBool("IsAttacking", active);
    public void TriggerAttackAnim(int index)    => photonView.RPC(nameof(RPC_PlayAttackAnimation), RpcTarget.All, index);
    public void TriggerHitAnim()                => photonView.RPC(nameof(RPC_PlayHitAnimation), RpcTarget.All);
    public void TriggerDeathAnim()              => photonView.RPC(nameof(RPC_PlayDeathAnimation), RpcTarget.All);

    // ─── [PunRPC] 수신 메서드 (Animator 실제 호출) ────────────────

    [PunRPC]
    private void RPC_PlayAttackAnimation(int index) => Animator.SetTrigger($"Attack{index}");

    [PunRPC]
    private void RPC_PlayHitAnimation() => Animator.SetTrigger("Hit");

    [PunRPC]
    private void RPC_PlayDeathAnimation() => Animator.SetTrigger("Death");

    [PunRPC]
    private void RPC_Respawn(Vector3 position)
    {
        transform.position = position;
        Animator.Rebind();
        Animator.Update(0f);
    }

    // ─── 공격 데미지 적용 (BearAttackState에서 호출) ─────────────

    // TODO : 히트박스에서 공격 처리 담당하기
    public void ApplyDamageToTarget()
    {
        if (Target == null) return;

        PhotonView targetView = Target.GetComponent<PhotonView>();
        if (targetView == null) return;

        targetView.RPC("TakeDamage", RpcTarget.All,
            _stat.Damage.Value,
            photonView.Owner.ActorNumber);
    }

    // ─── 리스폰 코루틴 (BearDeathState에서 StartCoroutine으로 시작) ─

    public IEnumerator DeathRespawn_Coroutine()
    {
        ItemObjectFactory.Instance.RequestMakeScoreItem(transform.position);

        yield return new WaitForSeconds(_respawnDelay);

        _stat.Health.Initialize();
        photonView.RPC(nameof(RPC_Respawn), RpcTarget.All, BasePosition);

        Agent.enabled = true;
        Agent.Warp(BasePosition);

        ChangeState(new BearIdleState(this));
    }
}
