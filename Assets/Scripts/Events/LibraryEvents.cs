using BaXoai;

namespace LibraryGame
{
    public struct BookPlacedEvent : IEvent
    {
        public BookObject Book;
        public BookShelf  Shelf;
        public bool       IsCorrect; // true = sách đúng bộ với kệ
    }

    public struct ShelfCompletedEvent : IEvent
    {
        public BookShelf Shelf; // kệ vừa full đúng bộ hoàn toàn
    }
}
