using System;
using Photon.Pun;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMoveAbility : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 7f;
    [SerializeField] private float _jumpForce = 2.5f;
    
    // 참조
    private CharacterController _characterController;
    private Animator _animator;
    private Camera _cam;
    
    // 상수
    private const float GRAVITY = 9.8f;
    private float _yVeocity = 0f;
    
    // 프로퍼티
    public float MoveSpeed => _moveSpeed;
    public float JumpForce => _jumpForce;


    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        _animator =  GetComponent<Animator>();
        _cam = Camera.main;
    }

    private void Update()
    {
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
            _yVeocity = JumpForce;
        }
       
      
        _characterController.Move(direction * _moveSpeed * Time.deltaTime);
        
       
    }
}
