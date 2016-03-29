using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WMPLib;

namespace MediaPlayer
{
    class Player
    {
        private WindowsMediaPlayer basePlayer;
        private List<MediaFile> _playlist;
        private List<MediaFile> _history;
        private List<MediaFile> _items;
        public WMPPlayState state;
        private MainWindow main;

        public Player(MainWindow m)
        {
            //Initializing
            basePlayer = new WindowsMediaPlayer();
            _playlist = new List<MediaFile>();
            _history = new List<MediaFile>();
            _items = new List<MediaFile>();

            //Some adjustments
            main = m;
            basePlayer.settings.volume = 100;

            //Events
            basePlayer.PlayStateChange += BasePlayer_PlayStateChange;
            basePlayer.MediaChange += BasePlayer_MediaChange;
        }

        private void BasePlayer_MediaChange(object Item)
        {
            if(Playlist.Count > 0)
            {
                //aktuelles item je nach umsetzung 0 oder 1 in playlist
                //Länge vom Item in LblTimeLeft schreiben

                //Playlist.RemoveAt(0);
                //MediaFile currItem = Playlist[0]; //temp
                Console.WriteLine(Playlist[0].Path);
                main.SetNewTrack(HistoryList[0], Playlist[0]);
                main.TimelineMaximum = basePlayer.currentMedia.duration;
                TimeHandler();
            }
        }

        private void BasePlayer_PlayStateChange(int NewState)
        {
            state = basePlayer.playState;
            Console.WriteLine(basePlayer.playState);

            switch (basePlayer.playState)
            {
                case WMPPlayState.wmppsPlaying:
                    main.PlayIcon = PackIconKind.Pause;
                    break;

                case WMPPlayState.wmppsPaused:
                    main.PlayIcon = PackIconKind.Play;
                    break;

                case WMPPlayState.wmppsStopped:
                    if (Playlist.Count > 0)
                    {
                        basePlayer.URL = Playlist[0].Path;
                        basePlayer.controls.play();
                    }
                    else main.PlayIcon = PackIconKind.Stop;
                    break;

                default:

                    break;
            }
        }

        public List<MediaFile> HistoryList
        {
            get
            {
                return _history;
            }
            set
            {
                _history = value;
            }
        }

        public List<MediaFile> Itemlist
        {
            get
            {
                return _items;
            }
            set
            {
                _items = value;
            }
        }

        public void Pause()
        {
            basePlayer.controls.pause();
        }

        public void Play()
        {
            if(basePlayer.URL == string.Empty)
                basePlayer.URL = _playlist[0].Path;
            basePlayer.controls.play();
        }

        public List<MediaFile> Playlist
        {
            get
            {
                return _playlist;
            }
            set
            {
                _playlist = value;
            }
        }

        public void SkipNext()
        {
            //basePlayer.controls.next();
            this.Stop();
            if (HistoryList.Count >= 50)
                HistoryList.RemoveAt(HistoryList.Count - 1);
            HistoryList.Insert(0, Playlist[0]);
            Playlist.RemoveAt(0);
            basePlayer.URL = Playlist[0].Path;
            this.Play();
        }

        public void SkipPrevious()
        {
            this.Stop();
            if(HistoryList.Count > 0)
            {
                Playlist.Insert(0, HistoryList[0]);
                HistoryList.RemoveAt(0);
                basePlayer.URL = Playlist[0].Path;
                this.Play();
            }
        }

        public void Stop()
        {
            basePlayer.URL = String.Empty;
            basePlayer.controls.stop();
        }

        private async void TimeHandler()
        {
            TimeSpan timeGone = TimeSpan.FromSeconds(basePlayer.controls.currentPosition);

            while (timeGone < TimeSpan.FromSeconds(basePlayer.currentMedia.duration))
            {
                timeGone = TimeSpan.FromSeconds(basePlayer.controls.currentPosition);
                main.TimelineValue = basePlayer.controls.currentPosition;

                if (timeGone.Ticks > ((basePlayer.currentMedia.duration - 5) * TimeSpan.TicksPerSecond) && Playlist.Count > 1)
                {
                    //Zuweisung / aktivierung des nächsten Tracks
                }

                await Task.Delay(50);
            }
        }

        public int Volume
        {
            get
            {
                return basePlayer.settings.volume;
            }
            set
            {
                basePlayer.settings.volume = value;
            }
        }
    }
}
