using System;
using Photon.Pun;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMoveAbility : PlayerAbility
{
    [SerializeField] private float _staminaForJump = 10f;
    
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
        if (Input.GetKey(KeyCode.Space) && _characterController.isGrounded && _owner.CurrentStamina > _staminaForJump)
        {
            _yVeocity = _owner.PlayerStat.JumpPower;
            _owner.CurrentStamina = Mathf.Clamp(_owner.CurrentStamina -  _staminaForJump, 0, _owner.MaxStamina);
            _owner.OnStaminaChanged.Invoke();
        }
       
        direction.y = _yVeocity;
        
        // 달리기
        if (Input.GetKey(KeyCode.LeftShift) && _owner.CurrentStamina > 0)
        {
            if (!_characterController.isGrounded) return;
            
            _owner.CurrentStamina -= _owner.PlayerStat.Stamina * Time.deltaTime;
            _owner.CurrentStamina = Mathf.Max(0, _owner.CurrentStamina);
            _characterController.Move(direction * Time.deltaTime * _owner.PlayerStat.RunSpeed);
            _owner.OnStaminaChanged.Invoke();
        }
        else
        {
            _owner.CurrentStamina += _owner.PlayerStat.Stamina * Time.deltaTime;
            _owner.CurrentStamina = Mathf.Min(_owner.CurrentStamina, _owner.MaxStamina);
            _characterController.Move(direction * Time.deltaTime * _owner.PlayerStat.MoveSpeed);
            _owner.OnStaminaChanged.Invoke();
        }
       
    }
}
