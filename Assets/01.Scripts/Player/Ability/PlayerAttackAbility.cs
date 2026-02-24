using System;
using Photon.Pun;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerAttackAbility : PlayerAbility
{
    private enum EAttackOption
    {
        Seq = 0,
        Random = 1,
    }

    [SerializeField] private EAttackOption _attackOption;
    [SerializeField] private float _staminaForAttack = 15f;
    
    private Animator _animator;
    private PhotonView _photonView;
    
    private float _attackTimer = 0f;
    private int _attackIndex = 0;

    private const int AttackMotionCnt = 3;
    
    protected override void Awake()
    {
        base.Awake();
        _animator = GetComponent<Animator>();
        _photonView = GetComponent<PhotonView>();
    }

    private void Update()
    {
        if (!_photonView.IsMine) return;
        
        _attackTimer += Time.deltaTime;
        
        if (Input.GetMouseButtonDown(0) &&  _attackTimer >= _owner.PlayerStat.AttackSpeed)
        {
           TryAttack();
        }
    }

    private void TryAttack()
    {
        if (_owner.CurrentStamina < _staminaForAttack) return;
        
        _attackTimer = 0f;
        _owner.CurrentStamina = Mathf.Clamp(_owner.CurrentStamina - _staminaForAttack, 0, _owner.MaxStamina);
        _owner.OnStaminaChanged.Invoke();
        
        Attack();
    }
    private void Attack()
    {
        string attackAnimation = "";
        
        if (_attackOption == EAttackOption.Seq)
        {
             attackAnimation = "Attack" + _attackIndex;
            _attackIndex = (_attackIndex + 1) % AttackMotionCnt;
        }
        else
        {
            _attackIndex = Random.Range(0, AttackMotionCnt);
             attackAnimation = "Attack" + _attackIndex;
        }

       
        _animator.SetTrigger(attackAnimation);
    }
}
