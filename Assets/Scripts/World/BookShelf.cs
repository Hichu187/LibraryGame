using System;
using System.Collections.Generic;
using UnityEngine;

namespace LibraryGame
{
    public class BookShelf : MonoBehaviour
    {
        [Serializable]
        public class BookSlot
        {
            [SerializeField] Transform _anchor;
            [SerializeField] Book _book;

            public Transform Anchor  => _anchor;
            public Book      Book    => _book;
            public bool      IsEmpty => _book == null;

            public void SetBook(Book book) => _book = book;
        }

        [SerializeField] BookSeries _series;
        [SerializeField] List<BookSlot> _slots = new();

        public BookSeries             Series  => _series;
        public IReadOnlyList<BookSlot> Slots  => _slots;

        // true nếu tất cả slot đều có sách
        public bool IsComplete
        {
            get
            {
                foreach (var slot in _slots)
                    if (slot.IsEmpty) return false;
                return true;
            }
        }

        public int EmptySlotCount
        {
            get
            {
                int count = 0;
                foreach (var slot in _slots)
                    if (slot.IsEmpty) count++;
                return count;
            }
        }

        // Tìm slot theo số thứ tự quyển
        public BookSlot GetSlotByVolume(int volumeNumber)
        {
            foreach (var slot in _slots)
                if (!slot.IsEmpty && slot.Book.VolumeNumber == volumeNumber)
                    return slot;
            return null;
        }

        // Tìm slot theo Book SO
        public BookSlot GetSlotByBook(Book book)
        {
            foreach (var slot in _slots)
                if (slot.Book == book)
                    return slot;
            return null;
        }

        // Tìm slot trống gần nhất với điểm va chạm (dùng cho preview placement)
        public BookSlot GetClosestEmptySlot(Vector3 worldPoint)
        {
            BookSlot closest = null;
            float minSqr = float.MaxValue;
            foreach (var slot in _slots)
            {
                if (!slot.IsEmpty || slot.Anchor == null) continue;
                float sqr = (slot.Anchor.position - worldPoint).sqrMagnitude;
                if (sqr < minSqr) { minSqr = sqr; closest = slot; }
            }
            return closest;
        }

#if UNITY_EDITOR
        void OnValidate()
        {
            if (_slots.Count < 10 || _slots.Count > 15)
                Debug.LogWarning($"[BookShelf] '{name}': mỗi bộ sách nên có 10–15 quyển (hiện tại: {_slots.Count}).", this);

            if (_series == null) return;

            foreach (var slot in _slots)
            {
                if (slot.IsEmpty) continue;
                if (slot.Book.Series != _series)
                    Debug.LogWarning($"[BookShelf] '{name}': '{slot.Book.DisplayName}' không thuộc bộ '{_series.SeriesName}'.", this);
            }
        }
#endif
    }
}
