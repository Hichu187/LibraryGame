using UnityEngine;

namespace LibraryGame
{
    [CreateAssetMenu(fileName = "NewBook", menuName = "LibraryGame/Book")]
    public class Book : ScriptableObject
    {
        [SerializeField] string _title;
        [SerializeField] string _author;
        [SerializeField] BookSeries _series;
        [SerializeField] int _volumeNumber;
        [SerializeField, TextArea(3, 6)] string _description;
        [SerializeField] Sprite _cover;
        [SerializeField] TextAsset _content;

        public string     Title        => _title;
        public string     Author       => _author;
        public BookSeries Series       => _series;
        public int        VolumeNumber => _volumeNumber;
        public string     Description  => _description;
        public Sprite     Cover        => _cover;
        public TextAsset  Content      => _content;

        public string DisplayName =>
            _series != null ? $"{_series.SeriesName} — Quyển {_volumeNumber}: {_title}" : _title;
    }
}
