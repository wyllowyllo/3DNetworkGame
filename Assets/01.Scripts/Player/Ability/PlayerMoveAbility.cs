using Photon.Pun;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMoveAbility : PlayerAbility
{
    [SerializeField] private float _staminaForJump = 10f;
    [SerializeField] private float _runDrainRate = 100f;  // 달리기 초당 스태미나 소모

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
        _animator            = GetComponent<Animator>();
        _cam                 = Camera.main;
        _photonView          = GetComponent<PhotonView>();
    }

    private void Update()
    {
        if (!_photonView.IsMine) return;
        if(_owner.IsDead) return;
        
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 direction = new Vector3(h, 0, v);
        direction.Normalize();
        direction = _cam.transform.TransformDirection(direction);

        _animator.SetFloat("Speed", direction.magnitude);

        // 중력
        _yVeocity -= GRAVITY * Time.deltaTime;

        // 점프
        if (Input.GetKey(KeyCode.Space) && _characterController.isGrounded && _owner.HasEnoughStamina(_staminaForJump))
        {
            _yVeocity = _owner.PlayerStat.JumpPower.Value;
            _owner.DrainStamina(_staminaForJump);
        }

        direction.y = _yVeocity;

        bool running = Input.GetKey(KeyCode.LeftShift) && _characterController.isGrounded && _owner.HasEnoughStamina(0.01f);
        if (running)
            _owner.DrainStamina(_runDrainRate * Time.deltaTime);
        else
            _owner.RegenStamina(Time.deltaTime);

        float speed = running ? _owner.PlayerStat.RunSpeed.Value : _owner.PlayerStat.MoveSpeed.Value;
        _characterController.Move(direction * Time.deltaTime * speed);
    }
}
