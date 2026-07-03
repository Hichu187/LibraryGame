using UnityEngine;

namespace LibraryGame
{
    [CreateAssetMenu(fileName = "NewBookSeries", menuName = "LibraryGame/Book Series")]
    public class BookSeries : ScriptableObject
    {
        [SerializeField] string _seriesName;
        [SerializeField] string _description;
        [SerializeField] Sprite _cover;

        public string SeriesName   => _seriesName;
        public string Description  => _description;
        public Sprite Cover        => _cover;
    }
}
