using System;
using Photon.Pun;
using Unity.Android.Gradle.Manifest;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMoveAbility : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 7f;
    [SerializeField] private float _jumpForce = 2.5f;
    
    // 참조
    private CharacterController _characterController;
    
    // 상수
    private const float GRAVITY = 9.8f;

    // 프로퍼티
    public float MoveSpeed => _moveSpeed;
    public float JumpForce => _jumpForce;


    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        float h= Input.GetAxis("Horizontal");
        float v= Input.GetAxis("Vertical");
        
        Vector3 direction = new Vector3(h, 0, v);
        direction.Normalize();
        
        // 중력
        direction.y -= GRAVITY * Time.deltaTime;
        
        // 점프
        if (Input.GetKey(KeyCode.Space) && _characterController.isGrounded)
        {
            direction.y = JumpForce;
        }
       
        
        _characterController.Move(direction * _moveSpeed * Time.deltaTime);
    }
}
