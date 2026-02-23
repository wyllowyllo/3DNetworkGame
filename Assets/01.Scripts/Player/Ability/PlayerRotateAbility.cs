using System;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem.Controls;

public class PlayerRotateAbility : PlayerAbility
{
    [SerializeField] private Transform _cameraRoot;
    
    private Camera _cam;
    
    private float _mx;
    private float _my;

    
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        
        CinemachineCamera vcam = GameObject.Find("FollowCamera").GetComponent<CinemachineCamera>();
        vcam.Follow = _cameraRoot.transform;
    }

    private void Update()
    {
        float _rotationSpeed = _owner.PlayerStat.RotationSpeed;
        _mx += Input.GetAxis("Mouse X") * _rotationSpeed * Time.deltaTime;
        _my += Input.GetAxis("Mouse Y") * _rotationSpeed * Time.deltaTime;
        
        _my = Mathf.Clamp(_my, -90f, 90f);
        
        transform.eulerAngles = new Vector3(0f, _mx, 0f);
        _cameraRoot.localEulerAngles = new Vector3(-_my, 0f, 0f);
    }
}
