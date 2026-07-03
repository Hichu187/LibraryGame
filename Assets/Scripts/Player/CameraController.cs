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
        [SerializeField] InputAction _cursorToggleAction;

        float _pitch;

        void Awake()
        {
            // Tự tìm PlayerBody nếu chưa gán trong Inspector (camera phải là con của Player)
            if (_playerBody == null)
                _playerBody = transformCached.parent;

            SetupDefaultBindings();
        }

        void OnEnable()
        {
            _lookAction.Enable();
            _cursorToggleAction.Enable();
        }

        void OnDisable()
        {
            _lookAction.Disable();
            _cursorToggleAction.Disable();
        }

        void Start()
        {
            LockCursor(true);
        }

        void Update()
        {
            //if (_cursorToggleAction.WasPressedThisFrame())
                //LockCursor(Cursor.lockState != CursorLockMode.Locked);

            //if (Cursor.lockState != CursorLockMode.Locked) return;

            Vector2 delta = _lookAction.ReadValue<Vector2>() * _sensitivity;
            _pitch = Mathf.Clamp(_pitch - delta.y, _pitchMin, _pitchMax);
            transformCached.localRotation = Quaternion.Euler(_pitch, 0f, 0f);
            _playerBody.Rotate(Vector3.up * delta.x);
        }

        void LockCursor(bool locked)
        {
            // Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
            // Cursor.visible = !locked;
        }

        void SetupDefaultBindings()
        {
            if (_lookAction.bindings.Count == 0)
                _lookAction.AddBinding("<Mouse>/delta");

            if (_cursorToggleAction.bindings.Count == 0)
                _cursorToggleAction.AddBinding("<Keyboard>/escape");
        }
    }
}
