using UnityEngine;
using UnityEngine.InputSystem;
using BaXoai;

namespace LibraryGame
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoCached
    {
        [Header("Di chuyển")]
        [SerializeField] float _moveSpeed = 4f;
        [SerializeField] float _sprintSpeed = 7f;

        [Header("Nhảy")]
        [SerializeField] float _jumpHeight = 1.2f;
        [SerializeField] float _gravity = -25f;
        [SerializeField] float _fallMultiplier = 2.5f;

        [Header("Kiểm tra mặt đất")]
        [SerializeField] float _groundCheckRadius = 0.3f;
        [SerializeField] LayerMask _groundMask;

        [Header("Input")]
        [SerializeField] InputAction _moveAction;
        [SerializeField] InputAction _sprintAction;
        [SerializeField] InputAction _jumpAction;

        CharacterController _cc;
        float _verticalVelocity;

        void Awake()
        {
            _cc = GetComponent<CharacterController>();
            SetupDefaultBindings();
        }

        void OnEnable()
        {
            _moveAction.Enable();
            _sprintAction.Enable();
            _jumpAction.Enable();
        }

        void OnDisable()
        {
            _moveAction.Disable();
            _sprintAction.Disable();
            _jumpAction.Disable();
        }

        void Update()
        {
            bool grounded = IsGrounded();

            if (grounded && _verticalVelocity < 0f)
                _verticalVelocity = -2f;

            if (_jumpAction.WasPressedThisFrame() && grounded)
                _verticalVelocity = Mathf.Sqrt(_jumpHeight * -2f * _gravity);

            float fallMult = _verticalVelocity < 0f ? _fallMultiplier : 1f;
            _verticalVelocity += _gravity * fallMult * Time.deltaTime;

            Vector2 input = _moveAction.ReadValue<Vector2>();
            Vector3 move = transformCached.right * input.x + transformCached.forward * input.y;
            if (move.sqrMagnitude > 1f) move.Normalize();

            float speed = _sprintAction.IsPressed() ? _sprintSpeed : _moveSpeed;
            _cc.Move((move * speed + Vector3.up * _verticalVelocity) * Time.deltaTime);
        }

        bool IsGrounded()
        {
            if (_groundMask.value == 0)
                return _cc.isGrounded;

            Vector3 bottom = transformCached.position + _cc.center + Vector3.down * (_cc.height * 0.5f - _cc.radius);
            return Physics.CheckSphere(bottom, _groundCheckRadius + _cc.skinWidth, _groundMask);
        }

        void SetupDefaultBindings()
        {
            if (_moveAction.bindings.Count == 0)
            {
                _moveAction.AddCompositeBinding("2DVector")
                    .With("Up",    "<Keyboard>/w")
                    .With("Down",  "<Keyboard>/s")
                    .With("Left",  "<Keyboard>/a")
                    .With("Right", "<Keyboard>/d");
            }

            if (_sprintAction.bindings.Count == 0)
                _sprintAction.AddBinding("<Keyboard>/leftShift");

            if (_jumpAction.bindings.Count == 0)
                _jumpAction.AddBinding("<Keyboard>/space");
        }
    }
}
