using UnityEngine;
using BaXoai;

namespace LibraryGame
{
    public class PlacementFeedbackController : MonoBehaviour
    {
        [SerializeField] AudioConfig _correctPlaceSound;
        [SerializeField] AudioConfig _shelfCompleteSound;

        void OnEnable()
        {
            StaticBus<BookPlacedEvent>.Subscribe(OnBookPlaced);
            StaticBus<ShelfCompletedEvent>.Subscribe(OnShelfCompleted);
        }

        void OnDisable()
        {
            StaticBus<BookPlacedEvent>.Unsubscribe(OnBookPlaced);
            StaticBus<ShelfCompletedEvent>.Unsubscribe(OnShelfCompleted);
        }

        void OnBookPlaced(BookPlacedEvent e)
        {
            if (!e.IsCorrect) return;
            AudioManager.Play(_correctPlaceSound, pos: e.Book.transform);
        }

        void OnShelfCompleted(ShelfCompletedEvent e)
        {
            AudioManager.Play(_shelfCompleteSound, pos: e.Shelf.transform);
        }
    }
}
