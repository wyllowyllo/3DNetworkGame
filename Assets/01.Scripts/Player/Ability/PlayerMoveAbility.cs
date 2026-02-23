using System;
using Photon.Pun;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMoveAbility : PlayerAbility
{
    
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
        direction.y = _yVeocity;
        
        // 점프
        if (Input.GetKey(KeyCode.Space) && _characterController.isGrounded)
        {
            _yVeocity = _owner.PlayerStat.JumpPower;
        }
       
      
        _characterController.Move(direction * _owner.PlayerStat.MoveSpeed * Time.deltaTime);
        
       
    }
}
