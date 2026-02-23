using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerAttackAbility : MonoBehaviour
{
    private enum EAttackOption
    {
        Seq = 0,
        Random = 1,
    }

    [SerializeField] private EAttackOption _attackOption;
    private int _attackIndex = 0;
    
    private Animator _animator;

    private const int AttackMotionCnt = 3;
    
    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
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
