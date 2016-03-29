using System;
using TagLib;

namespace MediaPlayer
{
    public class MediaFile
    {
        private int _index;
        private string _title;
        private string[] _artists;
        private string _album;
        private string _path;
        private TimeSpan _duration;

        public MediaFile(string path)
        {
            File f = File.Create(path);
            if (f != null)
            {
                _title = f.Tag.Title;
                _artists = f.Tag.AlbumArtists;
                _album = f.Tag.Album;
                _path = path;
                _duration = f.Properties.Duration;
                _index = 0;
            }
        }

        public MediaFile(string path, int index)
        {
            File f = File.Create(path);
            if (f != null)
            {
                _title = f.Tag.Title;
                _artists = f.Tag.AlbumArtists;
                _album = f.Tag.Album;
                _path = path;
                _duration = f.Properties.Duration;
                _index = index;
            }
        }

        public string[] ToStringArray()
        {
            return new string[]
            {
                Index.ToString(),
                Title,
                Artist,
                Album,
                Duration.ToString(),
                Path
            };
        }

        public int Index
        {
            get
            {
                return _index;
            }
            set
            {
                _index = value;
            }
        }

        public string Title
        {
            get
            {
                return _title;
            }
            set
            {
                _title = value;
            }
        }

        public string[] GetArtistsArray()
        {
                return _artists;
        }

        public string Artist
        {
            get
            {
                if (_artists.Length > 0)
                {
                    string end = _artists[0];
                    if (_artists.Length > 1)
                        for (int i = 1; i < _artists.Length; i++)
                            end += "; " + _artists[i];
                    return end;
                }
                else return string.Empty;
            }
        }

        public string Album
        {
            get
            {
                return _album;
            }
            set
            {
                _album = value;
            }
        }

        public string Path
        {
            get
            {
                return _path;
            }
            set
            {
                _path = value;
            }
        }

        public string Duration
        {
            get
            {
                return _duration.ToString(@"mm\:ss");
            }
        }
    }
}
