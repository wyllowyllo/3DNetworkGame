using System.Collections;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.AI;

public class BearMonster : MonoBehaviourPunCallbacks, IPunObservable, IDamagable
{
    enum EBearState
    {
        Idle       = 0,
        Patrol     = 1,
        MoveReturn = 2,
        MoveAttack = 3,
        Attack     = 4,
        AttackWait = 5,
        Hit        = 6,
        Death      = 7
    }

    [SerializeField] private BearStat _stat;
    [SerializeField] private float _respawnDelay = 8f;

    private NavMeshAgent _agent;
    private Animator     _animator;

    private Vector3    _basePosition;
    private Transform  _target;
    private EBearState _currentState;

    // 타이머
    private float _idleTimer;
    private float _attackCooldownTimer;
    private float _attackWaitTimer;
    private float _hitRecoveryTimer;

    private const float HIT_RECOVERY_TIME = 0.8f;

    // 프록시 클라이언트 동기화 값
    private float _syncedHP;
    private int   _syncedState;
    private float _syncedSpeed;

    // ───────────────────────────────────────────
    // 초기화
    // ───────────────────────────────────────────

    private void Awake()
    {
        _agent    = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        _basePosition = transform.position;

        if (PhotonNetwork.IsMasterClient)
        {
            _stat.Health.Initialize();
            EnterState(EBearState.Idle);
        }
    }

    // ───────────────────────────────────────────
    // 업데이트
    // ───────────────────────────────────────────

    private void Update()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            UpdateFSM();
            float speed = (_agent != null && _agent.enabled) ? _agent.velocity.magnitude : 0f;
            _animator.SetFloat("Speed", speed);
        }
        else
        {
            _animator.SetFloat("Speed", _syncedSpeed);
        }
    }

    // ───────────────────────────────────────────
    // FSM
    // ───────────────────────────────────────────

    private void UpdateFSM()
    {
        switch (_currentState)
        {
            case EBearState.Idle:       UpdateIdle();       break;
            case EBearState.Patrol:     UpdatePatrol();     break;
            case EBearState.MoveReturn: UpdateMoveReturn(); break;
            case EBearState.MoveAttack: UpdateMoveAttack(); break;
            case EBearState.Attack:     UpdateAttack();     break;
            case EBearState.AttackWait: UpdateAttackWait(); break;
            case EBearState.Hit:        UpdateHit();        break;
            case EBearState.Death:                          break;
        }
    }

    private void EnterState(EBearState newState)
    {
        _currentState = newState;

        switch (newState)
        {
            case EBearState.Idle:
                _agent.isStopped = true;
                _agent.ResetPath();
                _idleTimer = Random.Range(2f, 5f);
                break;

            case EBearState.Patrol:
                _agent.isStopped = false;
                _agent.speed = _stat.MoveSpeed.Value;
                _agent.SetDestination(GetRandomPatrolPoint());
                break;

            case EBearState.MoveReturn:
                _target = null;
                _agent.isStopped = false;
                _agent.speed = _stat.MoveSpeed.Value;
                _agent.SetDestination(_basePosition);
                break;

            case EBearState.MoveAttack:
                _agent.isStopped = false;
                _agent.speed = _stat.RunSpeed.Value;
                break;

            case EBearState.Attack:
                _agent.isStopped = true;
                _attackCooldownTimer = _stat.AttackCooldown;
                ApplyAttackDamage();
                int attackIndex = GetRandomAttackIndex();
                photonView.RPC(nameof(RPC_PlayAttackAnimation), RpcTarget.All, attackIndex);
                break;

            case EBearState.AttackWait:
                _agent.isStopped = true;
                _attackWaitTimer = _stat.AttackCooldown;
                break;

            case EBearState.Hit:
                _agent.isStopped = true;
                _hitRecoveryTimer = HIT_RECOVERY_TIME;
                photonView.RPC(nameof(RPC_PlayHitAnimation), RpcTarget.All);
                break;

            case EBearState.Death:
                _agent.isStopped = true;
                _agent.enabled = false;
                photonView.RPC(nameof(RPC_PlayDeathAnimation), RpcTarget.All);
                StartCoroutine(DeathRespawn_Coroutine());
                break;
        }
    }

    // ───────────────────────────────────────────
    // 상태별 업데이트
    // ───────────────────────────────────────────

    private void UpdateIdle()
    {
        _idleTimer -= Time.deltaTime;

        DetectPlayer();
        if (_target != null)
        {
            EnterState(EBearState.MoveAttack);
            return;
        }

        if (_idleTimer <= 0f)
            EnterState(EBearState.Patrol);
    }

    private void UpdatePatrol()
    {
        DetectPlayer();
        if (_target != null)
        {
            EnterState(EBearState.MoveAttack);
            return;
        }

        if (Vector3.Distance(transform.position, _basePosition) > _stat.ReturnRange)
        {
            EnterState(EBearState.MoveReturn);
            return;
        }

        if (!_agent.pathPending && _agent.remainingDistance < 1f)
            EnterState(EBearState.Idle);
    }

    private void UpdateMoveReturn()
    {
        DetectPlayer();
        if (_target != null)
        {
            EnterState(EBearState.MoveAttack);
            return;
        }

        if (!_agent.pathPending && _agent.remainingDistance < 1f)
            EnterState(EBearState.Idle);
    }

    private void UpdateMoveAttack()
    {
        if (_target == null)
        {
            EnterState(EBearState.MoveReturn);
            return;
        }

        if (Vector3.Distance(transform.position, _basePosition) > _stat.ReturnRange)
        {
            EnterState(EBearState.MoveReturn);
            return;
        }

        _agent.SetDestination(_target.position);

        if (Vector3.Distance(transform.position, _target.position) <= _stat.AttackRange)
            EnterState(EBearState.Attack);
    }

    private void UpdateAttack()
    {
        _attackCooldownTimer -= Time.deltaTime;
        if (_attackCooldownTimer <= 0f)
            EnterState(EBearState.AttackWait);
    }

    private void UpdateAttackWait()
    {
        _attackWaitTimer -= Time.deltaTime;
        if (_attackWaitTimer > 0f) return;

        if (_target == null)
        {
            EnterState(EBearState.MoveReturn);
            return;
        }

        float dist = Vector3.Distance(transform.position, _target.position);
        EnterState(dist <= _stat.AttackRange ? EBearState.Attack : EBearState.MoveAttack);
    }

    private void UpdateHit()
    {
        _hitRecoveryTimer -= Time.deltaTime;
        if (_hitRecoveryTimer <= 0f)
            EnterState(EBearState.MoveAttack);
    }

    // ───────────────────────────────────────────
    // 공격 유틸리티
    // ───────────────────────────────────────────

    private void ApplyAttackDamage()
    {
        if (_target == null) return;

        PhotonView targetView = _target.GetComponent<PhotonView>();
        if (targetView == null) return;

        targetView.RPC("TakeDamage", RpcTarget.All,
            _stat.Damage.Value,
            photonView.Owner.ActorNumber);
    }

    private int GetRandomAttackIndex()
    {
        int[] indices = { 1, 2, 3, 5 };
        return indices[Random.Range(0, indices.Length)];
    }

    // ───────────────────────────────────────────
    // 탐지 & 순찰
    // ───────────────────────────────────────────

    private void DetectPlayer()
    {
        Collider[] hits = Physics.OverlapSphere(
            transform.position, _stat.DetectRange,
            LayerMask.GetMask("Player"));

        _target = null;
        float minDist = float.MaxValue;

        foreach (var col in hits)
        {
            float d = Vector3.Distance(transform.position, col.transform.position);
            if (d < minDist)
            {
                minDist  = d;
                _target  = col.transform;
            }
        }
    }

    private Vector3 GetRandomPatrolPoint()
    {
        for (int i = 0; i < 10; i++)
        {
            Vector3 randomDir = Random.insideUnitSphere * _stat.PatrolRadius;
            randomDir += _basePosition;

            if (NavMesh.SamplePosition(randomDir, out NavMeshHit hit, _stat.PatrolRadius, NavMesh.AllAreas))
                return hit.position;
        }

        return _basePosition;
    }

    // ───────────────────────────────────────────
    // IDamagable
    // ───────────────────────────────────────────

    [PunRPC]
    public void TakeDamage(float damage, int attackerActorNumber)
    {
        if (!PhotonNetwork.IsMasterClient) return;
        if (_currentState == EBearState.Death) return;

        _stat.Health.Consume(damage);

        if (_stat.Health.Value <= 0f)
            EnterState(EBearState.Death);
        else
            EnterState(EBearState.Hit);
    }

    // ───────────────────────────────────────────
    // 죽음 & 리스폰
    // ───────────────────────────────────────────

    private IEnumerator DeathRespawn_Coroutine()
    {
        ItemObjectFactory.Instance.RequestMakeScoreItem(transform.position);

        yield return new WaitForSeconds(_respawnDelay);

        _stat.Health.Initialize();
        photonView.RPC(nameof(RPC_Respawn), RpcTarget.All, _basePosition);

        _agent.enabled = true;
        _agent.Warp(_basePosition);

        EnterState(EBearState.Idle);
    }

    // ───────────────────────────────────────────
    // RPC 애니메이션 트리거
    // ───────────────────────────────────────────

    [PunRPC]
    private void RPC_PlayAttackAnimation(int index)
    {
        _animator.SetTrigger($"Attack{index}");
    }

    [PunRPC]
    private void RPC_PlayHitAnimation()
    {
        _animator.SetTrigger("Hit");
    }

    [PunRPC]
    private void RPC_PlayDeathAnimation()
    {
        _animator.SetTrigger("Death");
    }

    [PunRPC]
    private void RPC_Respawn(Vector3 position)
    {
        transform.position = position;
        _animator.Rebind();
        _animator.Update(0f);
    }

    // ───────────────────────────────────────────
    // IPunObservable (30 Hz)
    // ───────────────────────────────────────────

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(_stat.Health.Value);
            stream.SendNext((int)_currentState);
            float speed = (_agent != null && _agent.enabled) ? _agent.velocity.magnitude : 0f;
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

    // ───────────────────────────────────────────
    // MasterClient 전환 콜백
    // ───────────────────────────────────────────

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        _stat.Health.SetValue(_syncedHP);

        EBearState restoreState = (EBearState)_syncedState;
        // Death 상태에서 인수인계되면 Idle로 복원 (코루틴 재시작 대신 안전한 초기화)
        if (restoreState == EBearState.Death)
        {
            _stat.Health.Initialize();
            _agent.enabled = true;
            restoreState   = EBearState.Idle;
            photonView.RPC(nameof(RPC_Respawn), RpcTarget.All, _basePosition);
        }

        EnterState(restoreState);
    }
}
