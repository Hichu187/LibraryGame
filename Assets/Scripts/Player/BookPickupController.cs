using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using BaXoai;

namespace LibraryGame
{
    public class BookPickupController : MonoCached
    {
        [SerializeField] InteractionController _interactionCtrl;
        [SerializeField] Transform _handAnchor;

        [Header("Stack")]
        [SerializeField] int     _maxBooks    = 5;
        [SerializeField] float   _stackOffset = 0.03f;
        [SerializeField] Vector3 _heldScale   = new(0.75f, 0.75f, 0.75f);
        [SerializeField] float   _lerpSpeed   = 10f;

        [Header("Input")]
        [SerializeField] InputAction _pickupAction;
        [SerializeField] InputAction _dropAction;
        [SerializeField] InputAction _scrollAction;

        readonly List<BookObject> _heldBooks = new();

        public BookObject TopBook =>
            _heldBooks.Count > 0 ? _heldBooks[_heldBooks.Count - 1] : null;

        // Gọi bởi BookShelfPlacementController khi đặt sách — không kích hoạt Drop physics
        public void RemoveTopFromStack()
        {
            if (_heldBooks.Count > 0)
                _heldBooks.RemoveAt(_heldBooks.Count - 1);
        }

        void Awake() => SetupDefaultBindings();

        void OnEnable()
        {
            _pickupAction.Enable();
            _dropAction.Enable();
            _scrollAction.Enable();
        }

        void OnDisable()
        {
            _pickupAction.Disable();
            _dropAction.Disable();
            _scrollAction.Disable();
        }

        void Update()
        {
            HandlePickup();
            HandleDrop();
            HandleScroll();
            UpdateStackPositions();
        }

        void HandlePickup()
        {
            if (!_pickupAction.WasPressedThisFrame()) return;
            if (_heldBooks.Count >= _maxBooks) return;
            if (_interactionCtrl.CurrentTarget is not BookObject book) return;
            if (book.IsHeld) return;

            PickupBook(book);
        }

        void HandleDrop()
        {
            if (!_dropAction.WasPressedThisFrame()) return;
            if (_heldBooks.Count == 0) return;

            BookObject top = _heldBooks[_heldBooks.Count - 1];
            _heldBooks.RemoveAt(_heldBooks.Count - 1);
            top.Drop();
        }

        void HandleScroll()
        {
            if (_heldBooks.Count <= 1) return;

            float scroll = _scrollAction.ReadValue<float>();
            if (scroll == 0f) return;

            if (scroll > 0f)
            {
                // scroll lên: quyển trên cùng xuống đáy
                BookObject top = _heldBooks[_heldBooks.Count - 1];
                _heldBooks.RemoveAt(_heldBooks.Count - 1);
                _heldBooks.Insert(0, top);
            }
            else
            {
                // scroll xuống: quyển đáy lên trên cùng
                BookObject bottom = _heldBooks[0];
                _heldBooks.RemoveAt(0);
                _heldBooks.Add(bottom);
            }
        }

        void UpdateStackPositions()
        {
            float dt = Time.deltaTime;
            for (int i = 0; i < _heldBooks.Count; i++)
            {
                Vector3 target = Vector3.up * (_stackOffset * i);
                Transform t = _heldBooks[i].transform;
                t.localPosition = Vector3.Lerp(t.localPosition, target, _lerpSpeed * dt);
                t.localRotation = Quaternion.Lerp(t.localRotation, Quaternion.identity, _lerpSpeed * dt);
                t.localScale    = Vector3.Lerp(t.localScale, _heldScale, _lerpSpeed * dt);
            }
        }

        void PickupBook(BookObject book)
        {
            book.Pickup();
            book.transform.SetParent(_handAnchor);
            _heldBooks.Add(book);
        }

        void SetupDefaultBindings()
        {
            if (_pickupAction.bindings.Count == 0)
                _pickupAction.AddBinding("<Keyboard>/f");

            if (_dropAction.bindings.Count == 0)
                _dropAction.AddBinding("<Keyboard>/g");

            if (_scrollAction.bindings.Count == 0)
                _scrollAction.AddBinding("<Mouse>/scroll/y");
        }
    }
}
