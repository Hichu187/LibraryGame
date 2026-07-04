using UnityEngine;
using TMPro;
using BaXoai;

namespace LibraryGame
{
    public class GameHUD : MonoCached
    {
        [Header("Tiến trình")]
        [SerializeField] TextMeshProUGUI _progressLabel;
        [SerializeField] string          _progressFormat = "{0} / {1}";

        // --- Interaction hint (sẽ thêm sau) ---
        // [Header("Interaction Hint")]
        // [SerializeField] GameObject          _hintRoot;
        // [SerializeField] TextMeshProUGUI     _hintLabel;

        // --- Stack UI (sẽ thêm sau) ---
        // [Header("Stack")]
        // [SerializeField] Transform           _stackContainer;
        // [SerializeField] GameObject          _stackItemPrefab;

        void OnEnable()
        {
            GameManager.OnProgressChanged += OnProgressChanged;
        }

        void OnDisable()
        {
            GameManager.OnProgressChanged -= OnProgressChanged;
        }

        void Start()
        {
            var gm = FindFirstObjectByType<GameManager>();
            if (gm != null)
                OnProgressChanged(gm.CompletedCount, gm.TotalShelves);
            else
                SetProgressText(0, 0);
        }

        void OnProgressChanged(int completed, int total)
        {
            SetProgressText(completed, total);
        }

        void SetProgressText(int completed, int total)
        {
            if (_progressLabel == null) return;
            _progressLabel.text = string.Format(_progressFormat, completed, total);
        }
    }
}
