using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Data.Linq;
using System.Windows.Threading;
using Microsoft.Win32;

namespace MusicApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MediaPlayer mediaPlayer = new MediaPlayer();

        public MainWindow()
        {
            InitializeComponent();
            ReadFiles();
        }

        public void ReadFiles()
        {
            string folderPath = "C:\\Users\\Public\\Music"; //TODO: Store this variable somewhere else
            List<MusicTrack> tracks = new List<MusicTrack>();

            foreach (string file in Directory.EnumerateFiles(folderPath, "*.mp3"))
            {
                var tfile = TagLib.File.Create(file);
                MusicTrack track = new MusicTrack();             
                tracks.Add(new MusicTrack() {
                    Name = tfile.Tag.Title, 
                    Artists = tfile.Tag.Performers, 
                    Album = tfile.Tag.Album, 
                    FileLocation = file 
                });
                listViewData.ItemsSource = tracks;
            }
            CreateInitalPlaylist(tracks);
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(listViewData.ItemsSource);
            view.Filter = UserFilter;
            mediaPlayer.MediaEnded += (sender, eventArgs) => NextSong(); //TODO: Why is this here?
        }

        void CreateInitalPlaylist(List<MusicTrack> Tracks)
        {
            Playlist playlist = new Playlist();
            playlist.Name = "All Songs";
            playlist.Tracks = Tracks;
            Console.WriteLine(playlist);
            //listViewPlaylists.ItemsSource = playlist;
            //TODO: Push playlist into new list box and then create functionality to create new playlists
        }

        void AutoPlay()
        {
            listViewData.SelectedIndex = listViewData.SelectedIndex + 1;
            var file = listViewData.SelectedItem as MusicTrack;
            mediaPlayer.Open(new Uri(file.FileLocation));
            mediaPlayer.Play();
        }

        void ShuffleTracks()
        {
            // Fix this
        }


        private void btnPlayAll_Click(object sender, RoutedEventArgs e)
        {
            listViewData.SelectedIndex = 0;
            var file = listViewData.SelectedItem as MusicTrack;
            mediaPlayer.Open(new Uri(file.FileLocation));
            mediaPlayer.Play();
            showSongTimer();
        }

        private void btnSkip_Click(object sender, RoutedEventArgs e)
        {
            NextSong();
        }

        void NextSong()
        {
            listViewData.SelectedIndex = listViewData.SelectedIndex + 1;
            var file = listViewData.SelectedItem as MusicTrack;
            mediaPlayer.Open(new Uri(file.FileLocation));
            mediaPlayer.Play();
            showSongTimer();
        }

        void PreviousSong()
        {
            listViewData.SelectedIndex = listViewData.SelectedIndex - 1;
            var file = listViewData.SelectedItem as MusicTrack;
            mediaPlayer.Open(new Uri(file.FileLocation));
            mediaPlayer.Play();
            showSongTimer();
        }

        void prepareSong(object sender, MouseButtonEventArgs e)
        {
            var baseobj = sender as FrameworkElement;
            var musicTrack = baseobj.DataContext as MusicTrack;
            mediaPlayer.Open(new Uri(musicTrack.FileLocation));
            showSongTimer();

        }

        void showSongTimer()
        {
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += timer_Tick;
            timer.Start();
        }

        private void ListViewItem_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            prepareSong(sender, e);
        }

        void ListView_MouseDoubleClick(Object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                prepareSong(sender, e);
                mediaPlayer.Play();
            }
        }

        void timer_Tick(object sender, EventArgs e)
        {
            if (mediaPlayer.Source != null)
            {
                if (mediaPlayer.NaturalDuration.HasTimeSpan)
                {
                    var track = listViewData.SelectedItem as MusicTrack;
                    lblStatus.Content = String.Format("{0} - {1} / {2} : {3}", track.Name, track.Album, mediaPlayer.Position.ToString(@"mm\:ss"), mediaPlayer.NaturalDuration.TimeSpan.ToString(@"mm\:ss"));
                    
                } 
            } 
            else
            {
                lblStatus.Content = "No file selected...";
            }
                
        }

        private void btnPrevious_Click(object sender, RoutedEventArgs e)
        {
            PreviousSong();
        }

        private void btnPlay_Click(object sender, RoutedEventArgs e)
        {
            mediaPlayer.Play();
          
        }

        private void btnPause_Click(object sender, RoutedEventArgs e)
        {
            mediaPlayer.Pause();
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            mediaPlayer.Stop();
        }

        private void btnShuffle_Click(object sender, RoutedEventArgs e)
        {
            ShuffleTracks();
        }

        private bool UserFilter(object item)
        {
            if (String.IsNullOrEmpty(txtFilter.Text))
                return true;
            else
                return ((item as MusicTrack).Name.IndexOf(txtFilter.Text, StringComparison.OrdinalIgnoreCase) >= 0);
        }

        private void txtFilter_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(listViewData.ItemsSource).Refresh();
        }

        private void listView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void listViewData_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        
    }
}
