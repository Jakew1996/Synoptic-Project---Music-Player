using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace MusicApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MediaPlayer mediaPlayer = new MediaPlayer();
        public ObservableCollection<Playlist> Playlists { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            ReadFiles();



            var timer = new DispatcherTimer
            (
            TimeSpan.FromSeconds(300), //TODO: Change this to 30 seconds
            DispatcherPriority.ApplicationIdle,// Or DispatcherPriority.SystemIdle
            (s, e) => { 
                Idle();
            }, // or something similar
            Application.Current.Dispatcher
            );
        }

        public void ReadFiles()
        {

            string folderPath = "C:\\Users\\Public\\Music"; //TODO: Store this variable somewhere else

            List<MusicTrack> tracks = new List<MusicTrack>();

            foreach (string file in Directory.EnumerateFiles(folderPath, "*.mp3"))
            {
                var tfile = TagLib.File.Create(file);
                MusicTrack track = new MusicTrack();
                tracks.Add(new MusicTrack()
                {
                    Name = tfile.Tag.Title,
                    Artists = tfile.Tag.Performers.ElementAt(0),
                    Album = tfile.Tag.Album,
                    FileLocation = file
                });
                
            }

            
            //listViewData.ItemsSource = tracks;
            CreateInitalPlaylist(tracks);
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(listViewData.ItemsSource);
            //PropertyGroupDescription groupDescription = new PropertyGroupDescription("Name");
            //view.Filter = UserFilter;
            mediaPlayer.MediaEnded += (sender, eventArgs) => NextSong(); //TODO: Why is this here?
        }

        void CreateInitalPlaylist(List<MusicTrack> Tracks)
        {
            ObservableCollection<Playlist> playlist = new ObservableCollection<Playlist>();
            playlist.Add(new Playlist()
            {
                Name = "All Songs",
                Tracks = Tracks,
            });
            Playlists = playlist;
            listViewPlaylists.DataContext = this;



            //List<Playlist> playlist = new List<Playlist>();
            //listViewPlaylists.ItemsSource = playlist;
        }

        public void AddPlaylist()
        {
            PlaylistModal playlistModal = new PlaylistModal();
            Playlists.Add(new Playlist()
            {
                //Name = playlistModal.PlaylistName.Text,
                Name = NewPlaylistName.Text,
                Tracks = { },
            });
            Console.WriteLine(playlistModal.PlaylistName.Text);
            listViewPlaylists.DataContext = this;
        }






        private void ListViewPlaylist_ItemClick(object sender, MouseButtonEventArgs e)
        {
            Console.WriteLine(e);
            Console.WriteLine(sender);
            //var SelectedItem = sender.Content.tracks; //TODO: Fix this
            List<MusicTrack> tracks = new List<MusicTrack>();
            foreach (var item in SelectedItem.Tracks)
            {

                //var tfile = TagLib.File.Create(file);
                MusicTrack track = new MusicTrack();
                tracks.Add(new MusicTrack()
                {
                    Name = item.Name,
                    Artists = item.Artists,
                    Album = item.Album,
                    FileLocation = item.FileLocation
                });

            }
                listViewData.ItemsSource = tracks;
        }






        void Idle()
        {
            var darkwindow = new Window()
            {
                Background = Brushes.Black,
                Opacity = 0.4,
                AllowsTransparency = true,
                WindowStyle = WindowStyle.None,
                WindowState = WindowState.Maximized,
                Topmost = true
            };
            darkwindow.Show();
            string Idlemessage;

            var track = listViewData.SelectedItem as MusicTrack;
            if (track != null)
            {
                Idlemessage = String.Format("Now playing... {0} - {1}", track.Name, track.Album);
            }
            else
            {
                Idlemessage = "...";
            }
            MessageBox.Show(Idlemessage);
            darkwindow.Close();
        }

        void AutoPlay()
        {
            if (listViewData.SelectedIndex != 0)
            {
                listViewData.SelectedIndex = listViewData.SelectedIndex + 1;
            }
            var file = listViewData.SelectedItem as MusicTrack;
            mediaPlayer.Open(new Uri(file.FileLocation));
            mediaPlayer.Play();
            showSongTimer();
        }

        void PlayAll()
        {
            listViewData.SelectedIndex = 0;
            AutoPlay();
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

        
        void timer_Tick(object sender, EventArgs e)
        {
            if (mediaPlayer.Source != null)
            {
                if (mediaPlayer.NaturalDuration.HasTimeSpan)
                {
                    IdleModal idleModal = new IdleModal();
                    var track = listViewData.SelectedItem as MusicTrack;
                    lblStatusSongArtist.Content = String.Format("{0} - {1}", track.Name, track.Album);
                    lblStatus.Content = String.Format("{0} : {1}", mediaPlayer.Position.ToString(@"mm\:ss"), mediaPlayer.NaturalDuration.TimeSpan.ToString(@"mm\:ss"));
                }
            }
            else
            {
                lblStatus.Content = "...";
            }

        }
        private void txtFilter_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(listViewData.ItemsSource).Refresh();
        }

        void ShuffleMusic()
        {
            List<MusicTrack> tracks = listViewData.ItemsSource.Cast<MusicTrack>().ToList();
            var shuffledTracks = tracks.OrderBy(a => Guid.NewGuid()).ToList();
            listViewData.ItemsSource = shuffledTracks;
        }

        private bool UserFilter(object item)
        {
            if (String.IsNullOrEmpty(txtFilter.Text))
                return true;
            else
                return ((item as MusicTrack).Name.IndexOf(txtFilter.Text, StringComparison.OrdinalIgnoreCase) >= 0);
        }











        void ListViewPlaylist_MouseDoubleClick(Object sender, MouseButtonEventArgs e)
        {

        }

        private void btnPlayAll_Click(object sender, RoutedEventArgs e)
        {
            PlayAll();
        }

        private void btnSkip_Click(object sender, RoutedEventArgs e)
        {
            NextSong();
        }

        private void btnCreatePlaylist_Click(object sender, RoutedEventArgs e)
        {
            AddPlaylist();
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

        private void btnPrevious_Click(object sender, RoutedEventArgs e)
        {
            PreviousSong();
        }

        private void btnPlay_Click(object sender, RoutedEventArgs e)
        {
            mediaPlayer.Play();

        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            mediaPlayer.Pause(); // Have the pause method here as the functionality stops it, this is reversed with the pause button as the stop method pauses.
        }

        private void btnPause_Click(object sender, RoutedEventArgs e)
        {
            mediaPlayer.Stop();
        }

        private void btnShuffle_Click(object sender, RoutedEventArgs e)
        {
            ShuffleMusic();
        }

        private void listView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void listViewData_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void listViewPlaylists_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void NewPlaylistName_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
