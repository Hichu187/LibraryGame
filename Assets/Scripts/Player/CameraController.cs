using UnityEngine;
using UnityEngine.InputSystem;
using BaXoai;

namespace LibraryGame
{
    public class CameraController : MonoCached
    {
        [Header("Góc nhìn")]
        [SerializeField] Transform _playerBody;
        [SerializeField] float _sensitivity = 0.15f;
        [SerializeField] float _pitchMin = -80f;
        [SerializeField] float _pitchMax = 80f;

        [Header("Input")]
        [SerializeField] InputAction _lookAction;

        float _pitch;

        // UI gọi CameraController.SetUIActive(true) để tạm dừng camera khi mở menu
        public static bool IsUIActive { get; set; }

        void Awake()
        {
            if (_playerBody == null)
                _playerBody = transformCached.parent;

            SetupDefaultBindings();
        }

        void OnEnable()  => _lookAction.Enable();
        void OnDisable() => _lookAction.Disable();

        void Update()
        {
            if (IsUIActive) return;

            Vector2 delta = _lookAction.ReadValue<Vector2>() * _sensitivity;
            _pitch = Mathf.Clamp(_pitch - delta.y, _pitchMin, _pitchMax);
            transformCached.localRotation = Quaternion.Euler(_pitch, 0f, 0f);
            _playerBody.Rotate(Vector3.up * delta.x);
        }

        void SetupDefaultBindings()
        {
            if (_lookAction.bindings.Count == 0)
                _lookAction.AddBinding("<Mouse>/delta");
        }
    }
}
