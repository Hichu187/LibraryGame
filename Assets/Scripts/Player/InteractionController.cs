using UnityEngine;
using UnityEngine.InputSystem;
using System;
using BaXoai;

namespace LibraryGame
{
    public class InteractionController : MonoCached
    {
        [SerializeField] float _range = 3f;
        [SerializeField] LayerMask _interactableMask;

        [Header("Input")]
        [SerializeField] InputAction _interactAction;

        // UI hoặc hệ thống khác subscribe để hiện/ẩn hint
        public event Action<IInteractable> OnHoverEnter;
        public event Action<IInteractable> OnHoverExit;

        public IInteractable CurrentTarget => _currentTarget;

        IInteractable _currentTarget;

        void Awake() => SetupDefaultBindings();

        void OnEnable()  => _interactAction.Enable();
        void OnDisable() => _interactAction.Disable();

        void Update()
        {
            DetectTarget();
            HandleInteract();
        }

        void DetectTarget()
        {
            IInteractable newTarget = null;
            int mask = _interactableMask.value != 0 ? _interactableMask.value : Physics.DefaultRaycastLayers;

            if (Physics.Raycast(transformCached.position, transformCached.forward, out var hit, _range, mask))
                hit.collider.TryGetComponent(out newTarget);

            if (newTarget == _currentTarget) return;

            if (_currentTarget != null)
            {
                _currentTarget.OnHoverExit();
                OnHoverExit?.Invoke(_currentTarget);
            }

            _currentTarget = newTarget;

            if (_currentTarget != null)
            {
                _currentTarget.OnHoverEnter();
                OnHoverEnter?.Invoke(_currentTarget);
            }
        }

        void HandleInteract()
        {
            if (_interactAction.WasPressedThisFrame() && _currentTarget != null)
                _currentTarget.OnInteract();
        }

        void SetupDefaultBindings()
        {
            if (_interactAction.bindings.Count == 0)
                _interactAction.AddBinding("<Keyboard>/e");
        }
    }
}
