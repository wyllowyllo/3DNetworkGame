using System.Collections;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

/// <summary>
/// Bear FSM Context.
/// 상태 전환, 데미지 수신, 애니메이션 이벤트, 네트워크 동기화를 전담한다.
/// 게임 로직(이동/판정)은 각 BearXxxState에 위임한다.
/// </summary>
public class BearController : MonsterController, IPunObservable
{
    [SerializeField] private BearStat _stat;
    [SerializeField] private float    _respawnDelay = 8f;

    public BearStat   Stat   => _stat;
    public BearSensor Sensor { get; private set; }
    public bool  IsDead    => _stat.Health.Value <= 0f;

    public Transform Target => Sensor.Target;

    // 프록시 클라이언트 동기화 값
    private float _syncedHP;
    private int   _syncedState;
    private float _syncedSpeed;

    // ─── 초기화 ───────────────────────────────────────────────────

    protected override void Awake()
    {
        base.Awake();
        Sensor = GetComponent<BearSensor>();
    }

    private void Start()
    {
        BasePosition = transform.position;
        Sensor.Initialize(_stat.DetectRange);

        if (PhotonNetwork.IsMasterClient)
        {
            _stat.Health.Initialize();
            ChangeState(new BearIdleState(this));
        }
    }

    // ─── 업데이트 ─────────────────────────────────────────────────

    protected override void Update()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Sensor.Detect(); // 탐지 먼저, 그다음 상태 업데이트
            float speed = (nav != null && nav.enabled) ? nav.velocity.magnitude : 0f;
            Animator.SetFloat("Speed", speed);
        }
        else
        {
            Animator.SetFloat("Speed", _syncedSpeed);
        }

        base.Update(); // CurrentState.Update() — Detect() 이후 실행
    }

    // ─── IDamagable ───────────────────────────────────────────────
    // 데미지 수신, HP 처리, 상태 전환을 한 곳에서 전담한다.

    [PunRPC]
    public void TakeDamage(float damage, int attackerActorNumber)
    {
        if (!PhotonNetwork.IsMasterClient) return;
        if (IsDead) return;

        _stat.Health.Consume(damage);

        if (IsDead)
            ChangeState(new BearDeathState(this));
        else
            ChangeState(new BearHitState(this));
    }

    // ─── IPunObservable (30 Hz) ───────────────────────────────────

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(_stat.Health.Value);
            stream.SendNext(CurrentStateId);
            float speed = (nav != null && nav.enabled) ? nav.velocity.magnitude : 0f;
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
            nav.enabled = true;
            photonView.RPC(nameof(RPC_Respawn), RpcTarget.All, BasePosition);
            restoreState = EBearState.Idle;
        }

        ChangeState(CreateState(restoreState));
    }

    private IMonsterState CreateState(EBearState state)
    {
        return state switch
        {
            EBearState.Idle     => new BearIdleState(this),
            EBearState.Patrol   => new BearPatrolState(this),
            EBearState.Comeback => new BearComebackState(this),
            EBearState.Approach => new BearApproachState(this),
            EBearState.Attack   => new BearAttackState(this),
            EBearState.Hit      => new BearHitState(this),
            EBearState.Death    => new BearDeathState(this),
            _                   => new BearIdleState(this),
        };
    }

    // ─── [PunRPC] 애니메이션 수신 메서드 ─────────────────────────

    [PunRPC]
    public void PlayAttackAnimation() => Animator.SetTrigger("Attack");

    [PunRPC]
    public void PlayHitAnimation() => Animator.SetTrigger("Hit");

    [PunRPC]
    public void PlayDeathAnimation() => Animator.SetTrigger("Death");

    [PunRPC]
    private void RPC_Respawn(Vector3 position)
    {
        transform.position = position;
        Animator.Rebind();
        Animator.Update(0f);
    }

    // ─── 리스폰 코루틴 (BearDeathState에서 StartCoroutine으로 시작) ─

    public IEnumerator DeathRespawn_Coroutine()
    {
        ItemObjectFactory.Instance.RequestMakeScoreItem(transform.position);

        yield return new WaitForSeconds(_respawnDelay);

        _stat.Health.Initialize();
        photonView.RPC(nameof(RPC_Respawn), RpcTarget.All, BasePosition);

        nav.enabled = true;
        nav.Warp(BasePosition);

        ChangeState(new BearIdleState(this));
    }
}
