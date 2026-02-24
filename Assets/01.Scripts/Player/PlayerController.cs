using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;


public class PlayerController : MonoBehaviour, IPunObservable
{
    [SerializeField] private PlayerStat _playerStat;
    private float _currentHealth;
    private float _currentStamina;
    
    // 이벤트
    public UnityEvent OnHealthChanged = new UnityEvent();
    public UnityEvent OnStaminaChanged = new UnityEvent();
    
    // 참조
    private PhotonView _photonView;
    
    // 프로퍼티
    public PlayerStat PlayerStat => _playerStat;
    public PhotonView PhotonView => _photonView;
    public float CurrentHealth { get => _currentHealth; set => _currentHealth = value; }
    public float CurrentStamina { get => _currentStamina; set => _currentStamina = value; }
    public float MaxHealth => _playerStat.Health;
    public float MaxStamina => _playerStat.Stamina;
    
    
    private Dictionary<Type, PlayerAbility> _abilitiesCache = new();

    private void Awake()
    {
        _photonView = GetComponent<PhotonView>();
        
        
        _currentHealth = _playerStat.Health;
        _currentStamina = _playerStat.Stamina;
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
            stream.SendNext(_currentHealth);
            stream.SendNext(_currentStamina);
        }
        else if (stream.IsReading)
        {
            
            _currentHealth = (float)stream.ReceiveNext(); // 준 순서대로 받는다
            _currentStamina = (float)stream.ReceiveNext(); // 준 순서대로 받는다

            OnHealthChanged.Invoke();
            OnStaminaChanged.Invoke();
        }
    }
}
