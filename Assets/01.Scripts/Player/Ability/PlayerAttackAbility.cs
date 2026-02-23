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
        
        if (Input.GetMouseButtonDown(0) &&  _attackTimer >= (1f / _owner.PlayerStat.AttackSpeed))
        {
            _attackTimer = 0f;
            Attack();
        }
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
