using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class PlayerController : MonoBehaviour, IPunObservable, IDamagable
{
    public float Score { get; set; }
    
    [SerializeField] private PlayerStat _playerStat;
    [SerializeField] private float _respawnDelay = 5f;

    // 이벤트
    public UnityEvent OnHealthChanged  = new UnityEvent();
    public UnityEvent OnStaminaChanged = new UnityEvent();

    // 참조
    private PhotonView _photonView;
    private Animator _animator;
    private CharacterController _characterController;
    
    // 프로퍼티
    public PlayerStat  PlayerStat      => _playerStat;
    public PhotonView  PhotonView      => _photonView;
    public float       CurrentHealth   => _playerStat.Health.Value;
    public float       CurrentStamina  => _playerStat.Stamina.Value;
    public float       MaxHealth       => _playerStat.Health.MaxValue;
    public float       MaxStamina      => _playerStat.Stamina.MaxValue;
    public bool        IsDead          => _playerStat.Health.Value <= 0;
    

    public bool HasEnoughStamina(float amount) => _playerStat.Stamina.Value >= amount;

    // 스태미나 메서드
    public void DrainStamina(float amount)
    {
        if(IsDead) return;
        
        _playerStat.Stamina.Consume(amount);
        OnStaminaChanged.Invoke();
    }

    public void RegenStamina(float deltaTime)
    {
        if(IsDead) return;
        
        _playerStat.Stamina.Regenerate(deltaTime);
        OnStaminaChanged.Invoke();
    }

    private Dictionary<Type, PlayerAbility> _abilitiesCache = new();

    private void Awake()
    {
        _photonView = GetComponent<PhotonView>();
        _animator = GetComponent<Animator>();
        _characterController = GetComponent<CharacterController>();
        
        _playerStat.Health.Initialize();
        _playerStat.Stamina.Initialize();
    }

    public T GetAbility<T>() where T : PlayerAbility
    {
        var type = typeof(T);

        if (_abilitiesCache.TryGetValue(type, out PlayerAbility ability))
        {
            return ability as T;
        }

        // 게으른 초기화/로딩 -> 처음에 곧바로 초기화/로딩을 하는게 아니라
        //                    필요할때만 하는.. 뒤로 미루는 기법
        ability = GetComponent<T>();

        if (ability != null)
        {
            _abilitiesCache[ability.GetType()] = ability;

            return ability as T;
        }

        throw new Exception($"어빌리티 {type.Name}을 {gameObject.name}에서 찾을 수 없습니다.");
    }

    // TODO : RPC 방식으로 변경
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        // 스트림 : '시냇물'처럼 데이터가 멈추지 않고 연속적으로 흐르는 데이터 흐름
        //       : 서버에서 주고받을 데이터가 담겨있는 변수

        // 읽기 / 쓰기 모드
        if (stream.IsWriting)
        {
            stream.SendNext(_playerStat.Health.Value);
            stream.SendNext(_playerStat.Stamina.Value);
            stream.SendNext(Score);
        }
        else if (stream.IsReading)
        {
            _playerStat.Health.SetValue((float)stream.ReceiveNext());   // 준 순서대로 받는다
            _playerStat.Stamina.SetValue((float)stream.ReceiveNext());  // 준 순서대로 받는다
            Score = (float)stream.ReceiveNext();

            OnHealthChanged.Invoke();
            OnStaminaChanged.Invoke();
        }
    }
    
    [PunRPC]
    public void TakeDamage(float damage, int attackerActorNumber)
    {
        if(IsDead) return;
        
        Debug.Log("피격당함!");
        
        _playerStat.Health.Consume(damage);
        OnHealthChanged.Invoke();

        if (IsDead)
        {
            _animator.SetTrigger("Die");

            PhotonRoomManager.Instance.OnPlayerDeath(attackerActorNumber, PhotonView.Owner.ActorNumber);
            
            StartCoroutine(Death_Coroutine());
        }
    }

   
    
    private IEnumerator Death_Coroutine()
    {
        _characterController.enabled = false;
        
        if (_photonView.IsMine)
        {
            // 아이템 생성
            ItemObjectFactory.Instance.RequestMakeScoreItem(transform.position);
            
            // 점수 갱신
            ScoreManager.Instance.DiscountScore();
        }
        
       yield return new WaitForSeconds(_respawnDelay);
       
       _playerStat.Health.Initialize();
       _playerStat.Stamina.Initialize();
       OnHealthChanged.Invoke();
       OnStaminaChanged.Invoke();
       
       
       _animator.SetTrigger("Revive");
       
       if (_photonView.IsMine)
       {
           var randomSpawnPoint = SpawnManager.Instance.GetRandomSpawnPosition();
           transform.position = randomSpawnPoint;
       }
      
       
       _characterController.enabled = true;
    }
}
