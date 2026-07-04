using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using BaXoai;

namespace LibraryGame
{
    public class BookShelfPlacementController : MonoCached
    {
        [SerializeField] BookPickupController _pickupCtrl;
        [SerializeField] float _range = 3f;
        [SerializeField] LayerMask _slotMask;
        [SerializeField] Camera _camera;

        [Header("Ghost")]
        [SerializeField] Material _ghostMaterial;

        [Header("Input")]
        [SerializeField] InputAction _placeAction;

        BookShelf          _targetShelf;
        BookShelf.BookSlot _targetSlot;
        GameObject         _ghostInstance;
        BookObject         _ghostSourceBook;

        void Awake()
        {
            if (_camera == null) _camera = Camera.main;
            SetupDefaultBindings();
        }

        void OnEnable()  => _placeAction.Enable();
        void OnDisable() => _placeAction.Disable();

        void Update()
        {
            DetectSlot();
            HandlePlace();
        }

        void DetectSlot()
        {
            if (_pickupCtrl.TopBook == null) { HideGhost(); return; }

            Transform cam = _camera.transform;
            if (!Physics.Raycast(cam.position, cam.forward, out var hit, _range, _slotMask))
            {
                HideGhost();
                return;
            }

            if (!hit.collider.TryGetComponent<BookSlotAnchor>(out var anchor) || !anchor.IsEmpty)
            {
                HideGhost();
                return;
            }

            _targetShelf = anchor.Shelf;
            _targetSlot  = anchor.Slot;
            ShowGhost(_pickupCtrl.TopBook, anchor.transform);
        }

        void ShowGhost(BookObject book, Transform anchor)
        {
            if (_ghostInstance == null || _ghostSourceBook != book)
            {
                HideGhost();
                _ghostInstance   = BuildGhost(book);
                _ghostSourceBook = book;
            }

            _ghostInstance.transform.position = anchor.position;
            _ghostInstance.transform.rotation = anchor.rotation;
        }

        void HideGhost()
        {
            if (_ghostInstance != null) { Destroy(_ghostInstance); _ghostInstance = null; }
            _ghostSourceBook = null;
            _targetShelf     = null;
            _targetSlot      = null;
        }

        void HandlePlace()
        {
            if (!_placeAction.WasPressedThisFrame()) return;
            if (_targetSlot == null || _targetShelf == null) return;

            BookObject book = _pickupCtrl.TopBook;
            if (book == null) return;

            _pickupCtrl.RemoveTopFromStack();
            book.Place(_targetShelf, _targetSlot);
            HideGhost();
        }

        // Tạo ghost bằng cách sao chép chỉ phần visual từ BookObject gốc
        GameObject BuildGhost(BookObject source)
        {
            var root = new GameObject("BookGhost");

            foreach (var srcRend in source.GetComponentsInChildren<MeshRenderer>())
            {
                var srcMf = srcRend.GetComponent<MeshFilter>();
                if (srcMf == null) continue;

                var child = new GameObject(srcRend.name);
                child.transform.SetParent(root.transform, false);

                // Vị trí tương đối so với root của source
                child.transform.localPosition =
                    source.transform.InverseTransformPoint(srcRend.transform.position);
                child.transform.localRotation =
                    Quaternion.Inverse(source.transform.rotation) * srcRend.transform.rotation;
                child.transform.localScale = srcRend.transform.lossyScale;

                child.AddComponent<MeshFilter>().sharedMesh = srcMf.sharedMesh;

                var mr   = child.AddComponent<MeshRenderer>();
                var mats = new Material[srcRend.sharedMaterials.Length];
                for (int i = 0; i < mats.Length; i++)
                    mats[i] = _ghostMaterial;
                mr.materials = mats;
            }

            return root;
        }

        void SetupDefaultBindings()
        {
            if (_placeAction.bindings.Count == 0)
                _placeAction.AddBinding("<Keyboard>/r");
        }
    }
}
