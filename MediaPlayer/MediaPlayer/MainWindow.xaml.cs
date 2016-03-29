using MaterialDesignThemes.Wpf;
using System.Windows;
using System.IO;
using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using WMPLib;
using System.Windows.Controls;
using System.Windows.Media;

namespace MediaPlayer
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private double mutedVolume;
        private Player player;

        public MainWindow()
        {
            InitializeComponent();
            player = new Player(this);
            mutedVolume = 0;
            DGTracklist.ItemsSource = player.Itemlist;
            SldVolume.Value = 100;
            player.Volume = 100;
            SldVolume.IsEnabled = false;
            //MaterialDesignThemes.Wpf.PaletteHelper x = new MaterialDesignThemes.Wpf.PaletteHelper();
        }

        private void SearchDir(string directory)
        {
            foreach (string subDirectory in Directory.GetDirectories(directory))
            {
                SearchDir(subDirectory);
                foreach (string file in Directory.GetFiles(subDirectory))
                    this.AddFile(file);
            }
        }

        private void AddFile(string path)
        {
            if (path.EndsWith(".mp3"))
            {
                //Just for checking the corruption
                TagLib.File tagFile = null;
                try
                {
                    tagFile = TagLib.File.Create(path);
                }
                catch (TagLib.CorruptFileException)
                {
                    return;
                }

                if (!tagFile.PossiblyCorrupt)
                {
                    MediaFile m = new MediaFile(path, player.Itemlist.Count + 1);
                    Console.WriteLine("Added " + m.Artist + " - " + m.Title);
                    player.Itemlist.Add(m);
                }
            }
        }

        internal void SetNewTrack(MediaFile previous, MediaFile next)
        {
            //unmark previous
            DataGridRow row = (DataGridRow)DGTracklist.ItemContainerGenerator.ContainerFromIndex(previous.Index -1);
            row.Background = Brushes.White;
            //mark next
            row = (DataGridRow)DGTracklist.ItemContainerGenerator.ContainerFromIndex(next.Index -1);
            row.Background = Brushes.LightGreen;
        }

        #region Events

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //PrepareDirectory(); Nein.
            SldVolume.IsEnabled = true;
            DGTracklist.Items.Refresh();
        }

        private void DGTracklist_DblClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            player.Stop();
            player.Playlist.Clear();
            int index = DGTracklist.SelectedIndex;

            for(int i = index; i > 0; i--)
            {
                player.HistoryList.Add(player.Itemlist[i]);
            }

            for(int i = index; i < player.Itemlist.Count; i++)
            {
                player.Playlist.Add(player.Itemlist[i]);
            }
            player.Play();
        }

        private async void BtnPlayPause_Click(object sender, RoutedEventArgs e)
        {
            if (player.Playlist.Count == 0)
            {
                Random rnd = new Random();
                player.Playlist.Add(player.Itemlist[rnd.Next(1, player.Itemlist.Count)]);
                player.Play();
            }
            else if (player.state == WMPPlayState.wmppsPlaying)
            {
                int volume = player.Volume;
                while (player.Volume > 0)
                {
                    player.Volume = player.Volume - 5;
                    await Task.Delay(5);
                }
                player.Pause();
                player.Volume = volume;
            }
            else if (player.state == WMPPlayState.wmppsPaused)
            {
                player.Play();
            }
        }

        private void SldVolume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (SldVolume.IsEnabled)
            {
                player.Volume = Convert.ToInt16(SldVolume.Value);
                if (SldVolume.Value == 0)
                    IcoMute.Kind = PackIconKind.VolumeOff;
                else if (SldVolume.Value < 33)
                    IcoMute.Kind = PackIconKind.VolumeLow;
                else if (SldVolume.Value > 66)
                    IcoMute.Kind = PackIconKind.VolumeHigh;
                else IcoMute.Kind = PackIconKind.VolumeMedium;
            }
        }

        private void BtnMute_Click(object sender, RoutedEventArgs e)
        {
            if (SldVolume.Value == 0)
            {
                SldVolume.Value = mutedVolume;
                mutedVolume = 0;
            }
            else
            {
                mutedVolume = SldVolume.Value;
                SldVolume.Value = 0;
            }
        }

        private void BtnSkipPrevious_Click(object sender, RoutedEventArgs e)
        {
            player.SkipPrevious();
        }

        private void BtnSkipNext_Click(object sender, RoutedEventArgs e)
        {
            player.SkipNext();
        }

#endregion

        #region Properties

        public double TimelineValue
        {
            get
            {
                return SldTimeline.Value;
            }
            set
            {
                LblTimeDown.Content = TimeSpan.FromSeconds(value).ToString(@"mm\:ss");
                SldTimeline.Value = value;
            }
        }

        public double TimelineMaximum
        {
            get
            {
                return SldTimeline.Maximum;
            }
            set
            {
                SldTimeline.Maximum = value;
            }
        }

        public double VolumeValue
        {
            get
            {
                return SldVolume.Value;
            }
            set
            {
                SldVolume.Value = value;
            }
        }

        public PackIconKind PlayIcon
        {
            set
            {
                IcoPlayPause.Kind = value;
            }
            get
            {
                return IcoPlayPause.Kind;
            }
        }
        #endregion

        private void BtnAddFolder_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog diag = new FolderBrowserDialog();
            diag.Description = "Please choose the Directory to import your music from.";
            if (diag.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                    SearchDir(diag.SelectedPath);
            }
        }

        private void BtnAddFiles_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            open.Multiselect = true;
            if (open.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                    foreach (string s in open.FileNames)
                        this.AddFile(s);
            }
        }

        private void SldTimeline_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //To set the Song Time Value on SldTimeline Clicked
            //Point p = e.GetPosition(SldTimeline);
            //Console.WriteLine("Maus Position: " + p.X + ", relativ vom Time-Slider.");
        }

        private void Window_KeyPressed(object sender, System.Windows.Input.KeyEventArgs e)
        {
            switch(e.Key)
            {
                case System.Windows.Input.Key.MediaPlayPause:

                    break;

                case System.Windows.Input.Key.MediaNextTrack:

                    break;

                case System.Windows.Input.Key.MediaPreviousTrack:

                    break;

                case System.Windows.Input.Key.MediaStop:

                    break;

                case System.Windows.Input.Key.Space:

                    break;
            }
        }
    }
}
