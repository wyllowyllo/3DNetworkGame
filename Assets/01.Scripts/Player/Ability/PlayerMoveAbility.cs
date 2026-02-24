using System;
using Photon.Pun;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMoveAbility : PlayerAbility
{
    [SerializeField] private float _staminaUnitPerSec = 1f;
    
    // 참조
    private CharacterController _characterController;
    private Animator _animator;
    private Camera _cam;
    private PhotonView _photonView;
    
    // 상수
    private const float GRAVITY = 9.8f;
    private float _yVeocity = 0f;
    

    protected override void Awake()
    {
        base.Awake();
        _characterController = GetComponent<CharacterController>();
        _animator =  GetComponent<Animator>();
        _cam = Camera.main;
        _photonView = GetComponent<PhotonView>();
    }

    private void Update()
    {
        if (!_photonView.IsMine) return;

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        
        Vector3 direction = new Vector3(h, 0, v);
        direction.Normalize();
        direction = _cam.transform.TransformDirection(direction);
        
        _animator.SetFloat("Speed", direction.magnitude);
        
        // 중력
        _yVeocity -= GRAVITY * Time.deltaTime;
      
        
        // 점프
        if (Input.GetKey(KeyCode.Space) && _characterController.isGrounded)
        {
            _yVeocity = _owner.PlayerPlayerStat.JumpPower;
        }
       
        direction.y = _yVeocity;
        
        // 달리기
        float moveSpeed = _owner.PlayerPlayerStat.MoveSpeed;
        if (Input.GetKey(KeyCode.LeftShift) && _owner.CurrentStamina > 0)
        {
            if (!_characterController.isGrounded) return;
            
            moveSpeed = _owner.PlayerPlayerStat.RunSpeed;
            _owner.CurrentStamina = Mathf.Clamp(_owner.CurrentStamina - _staminaUnitPerSec * Time.deltaTime, 0, _owner.MaxStamina);
            _owner.OnStaminaChanged.Invoke();
        }
        else
        {
            _owner.CurrentStamina = Mathf.Clamp(_owner.CurrentStamina + _staminaUnitPerSec * Time.deltaTime, 0, _owner.MaxStamina);
            _owner.OnStaminaChanged.Invoke();
        }
      
        _characterController.Move(direction * moveSpeed * Time.deltaTime);
        
       
    }
}
