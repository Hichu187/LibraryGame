using System;
using UnityEngine;

namespace LibraryGame
{
    public class BookObject : InteractableObject
    {
        [SerializeField] Book _book;
        [SerializeField] BookShelf _shelf;

        // UI và GameManager subscribe để xử lý khi sách được tương tác
        public static event Action<BookObject> OnInteracted;

        Collider         _collider;
        Rigidbody        _rb;
        Vector3          _originalScale;
        BookShelf.BookSlot _currentSlot;

        public Book      BookData  => _book;
        public BookShelf Shelf     => _shelf;
        public bool      IsOnShelf => _shelf != null;
        public bool      IsHeld    { get; private set; }

        public override string InteractionHint =>
            _book != null ? $"[E] Đọc  [F] Lấy: {_book.DisplayName}" : "[E] Đọc  [F] Lấy";

        protected override void Awake()
        {
            base.Awake();
            _collider      = GetComponent<Collider>();
            _rb            = GetComponent<Rigidbody>();
            _originalScale = transform.localScale;
        }

        public override void OnInteract() => OnInteracted?.Invoke(this);

        public void Pickup()
        {
            IsHeld = true;
            if (_rb != null) _rb.isKinematic = true;
            if (_collider != null) _collider.enabled = false;
            OnHoverExit();
            RemoveFromShelf();
        }

        public void Drop()
        {
            IsHeld = false;
            if (_collider != null) _collider.enabled = true;
            transform.SetParent(null);
            transform.localScale = _originalScale;
            if (_rb != null) _rb.isKinematic = false;
        }

        // Đặt sách vào slot cụ thể — gọi bởi BookShelfPlacementController
        public void Place(BookShelf shelf, BookShelf.BookSlot slot)
        {
            IsHeld = false;
            _currentSlot = slot;
            slot.SetBook(_book);

            if (_rb != null) _rb.isKinematic = true;
            if (_collider != null) _collider.enabled = true;

            transform.SetParent(slot.Anchor);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            transform.localScale    = _originalScale;

            PlaceOnShelf(shelf);
        }

        public void PlaceOnShelf(BookShelf shelf) => _shelf = shelf;

        public void RemoveFromShelf()
        {
            if (_currentSlot != null)
            {
                _currentSlot.SetBook(null);
                _currentSlot = null;
            }
            _shelf = null;
        }
    }
}
