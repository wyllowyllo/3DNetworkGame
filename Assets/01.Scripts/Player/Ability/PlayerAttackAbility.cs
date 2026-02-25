using Photon.Pun;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerAttackAbility : PlayerAbility
{
    private enum EAttackOption
    {
        Seq    = 0,
        Random = 1,
    }

    [SerializeField] private EAttackOption _attackOption;
    [SerializeField] private float _staminaForAttack = 15f;

    private Animator _animator;
    private PhotonView _photonView;

    private float _attackTimer = 0f;
    private int _prevAnimationNumber = 0;

    private const int AttackMotionCnt = 3;

    protected override void Awake()
    {
        base.Awake();
        _animator      = GetComponent<Animator>();
        _photonView    = GetComponent<PhotonView>();
    }

    private void Update()
    {
        if (!_photonView.IsMine) return;
        if(_owner.IsDead) return;

        _attackTimer += Time.deltaTime;

        if (Input.GetMouseButtonDown(0) && _attackTimer >= _owner.PlayerStat.AttackSpeed.Value)
        {
            TryAttack();
        }
    }

    private void TryAttack()
    {
        if (!_owner.HasEnoughStamina(_staminaForAttack)) return;
        _owner.DrainStamina(_staminaForAttack);

        _attackTimer = 0f;
        Attack();
    }

    private void Attack()
    {
        int animationNumber = 0;
        switch (_attackOption)
        {
            case EAttackOption.Seq:
            {
                animationNumber =  (_prevAnimationNumber++) % 3;
                break;
            }
                
            case EAttackOption.Random:
            {
                animationNumber = Random.Range(0, AttackMotionCnt);
                break;
            }
        }
            
        // 1. 일반 메서드 호출 방식
        PlayAttackAnimation(animationNumber);
            
        // 2. RPC 메서드 호출 방식
        // 다른 컴퓨터에 있는 내 플레이어 오브젝트의 PlayAttackAnimation 메서드를 실행한다.
        _owner.PhotonView.RPC(nameof(PlayAttackAnimation), RpcTarget.Others, animationNumber);
    }
    

    // 트랜스폼, 애니메이션(float 파라미터) 등과 같이 상시로 동기화가 필요한 데이터는 -> IPunObserble(OnPhotonSerializeView)
    // 애니메이션 트리거처럼 간헐적으로 특정한 이벤트가 발생했을 때만 변화하는 데이터 동기화는 데이터 동기화가 아닌 이벤트 동기화 -> RPC
    // RPC : Remote Procedure Call (원격 함수 호출)
    // ㄴ 물리적으로 떨어져 있는 다른 디바이스의 함수를 호출하는 기능
    
    // RPC로 호출할 함수는 반드시 [PunRPC] 어트리뷰트를 함수 앞에 명시해줘야 한다.
    [PunRPC]
    private void PlayAttackAnimation(int animationNumber)
    {
        _animator.SetTrigger($"Attack{animationNumber}");
    }
}
