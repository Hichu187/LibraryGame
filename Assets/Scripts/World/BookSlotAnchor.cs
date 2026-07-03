using UnityEngine;

namespace LibraryGame
{
    [RequireComponent(typeof(BoxCollider))]
    public class BookSlotAnchor : MonoBehaviour
    {
        [SerializeField] BookShelf _shelf;
        [SerializeField] int       _slotIndex;

        public BookShelf          Shelf   => _shelf;
        public BookShelf.BookSlot Slot    => _shelf != null && _slotIndex < _shelf.Slots.Count
                                             ? _shelf.Slots[_slotIndex] : null;
        public bool               IsEmpty => Slot?.IsEmpty ?? true;

        public void Init(BookShelf shelf, int index)
        {
            _shelf     = shelf;
            _slotIndex = index;
        }
    }
}
