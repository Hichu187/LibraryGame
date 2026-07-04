using System;
using System.Collections.Generic;
using UnityEngine;
using BaXoai;

namespace LibraryGame
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] List<BookShelf> _allShelves = new();

        public int TotalShelves    => _allShelves.Count;
        public int CompletedCount  { get; private set; }

        // UI subscribe để cập nhật HUD tiến trình
        public static event Action<int, int> OnProgressChanged;  // (completed, total)
        public static event Action           OnAllShelvesComplete;

        void OnEnable()
        {
            StaticBus<BookPlacedEvent>.Subscribe(OnBookPlaced);
        }

        void OnDisable()
        {
            StaticBus<BookPlacedEvent>.Unsubscribe(OnBookPlaced);
        }

        void OnBookPlaced(BookPlacedEvent e)
        {
            if (!e.IsCorrect) return;
            RefreshProgress();
        }

        void RefreshProgress()
        {
            int count = 0;
            foreach (var shelf in _allShelves)
                if (shelf.IsCorrectlyComplete) count++;

            CompletedCount = count;
            OnProgressChanged?.Invoke(CompletedCount, TotalShelves);

            if (CompletedCount == TotalShelves && TotalShelves > 0)
                OnAllShelvesComplete?.Invoke();
        }
    }
}
