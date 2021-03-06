﻿using System;
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

        PlayBackControls playBackControls = new PlayBackControls();
        public ObservableCollection<Playlist> Playlists { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            LoadMediaFiles();

            var timer = new DispatcherTimer
            (
                TimeSpan.FromSeconds(30), //TODO: Store this variable somewhere else
                DispatcherPriority.ApplicationIdle,
                (s, e) =>
                {
                    Idle();
                }, // or something similar
                Application.Current.Dispatcher
            );

            mediaPlayer.MediaEnded += (sender, eventArgs) => NextSong();
        }

        public void LoadMediaFiles()
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

            CreateInitalPlaylist(tracks);
            listViewData.ItemsSource = tracks;

            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(listViewData.ItemsSource);
            PropertyGroupDescription groupDescription = new PropertyGroupDescription("Name");
            view.Filter = UserFilter;
        }


        void CreateInitalPlaylist(List<MusicTrack> Tracks)
        {
            ObservableCollection<Playlist> playlist = new ObservableCollection<Playlist>
            {
                new Playlist()
                {
                    Name = "All Songs",
                    Tracks = Tracks,
                }
            };
            Playlists = playlist;
            listViewPlaylists.DataContext = this;
        }

        void CreatePlaylist()
        {
            Playlists.Add(new Playlist()
            {
                Name = NewPlaylistName.Text,
                Tracks = { },
            });
            listViewPlaylists.DataContext = this;
        }

        void ChangePlaylist(object sender)
        {
            List<MusicTrack> tracks = new List<MusicTrack>();

            var baseobj = sender as FrameworkElement;
            var playlist = baseobj.DataContext as Playlist;

            if (playlist.Tracks != null)
            {
                foreach (var item in playlist.Tracks)
                {
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
        }
        

        void AddToSongToPlaylist(Object sender)
        {
            //Currently only adds to the most recently created playlist
            var baseobj = sender as FrameworkElement;
            var musicTrack = baseobj.DataContext as MusicTrack;

            //GET LAST ITEM IN ARRAY
            var playlist = Playlists[Playlists.Count - 1];
            if (playlist.Tracks == null)
            {
                playlist.Tracks = new List<MusicTrack>();
            }
            playlist.Tracks.Add(musicTrack);
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
                Idlemessage = "Press 'ok' to wake";
            }
            MessageBox.Show(Idlemessage);
            darkwindow.Close();
        }

        public void PlaySong()
        {
            mediaPlayer.Play();
            showSongTimer();
        }

        void AutoPlay()
        {
            if (listViewData.SelectedIndex != 0)
            {
                listViewData.SelectedIndex = listViewData.SelectedIndex + 1;
            }
            var file = listViewData.SelectedItem as MusicTrack;
            mediaPlayer.Open(new Uri(file.FileLocation));
            PlaySong();
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
            PlaySong();
        }

        void PreviousSong()
        {
            listViewData.SelectedIndex = listViewData.SelectedIndex - 1;
            var file = listViewData.SelectedItem as MusicTrack;
            mediaPlayer.Open(new Uri(file.FileLocation));
            PlaySong();
        }

        void prepareSong(object sender, MouseButtonEventArgs e)
        {
            var baseobj = sender as FrameworkElement;
            var musicTrack = baseobj.DataContext as MusicTrack;
            mediaPlayer.Open(new Uri(musicTrack.FileLocation));

        }

        public void showSongTimer()
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
                    var track = listViewData.SelectedItem as MusicTrack;

                    if (track != null)
                    {
                        lblStatusSongArtist.Content = String.Format("{0} - {1}", track.Name, track.Album);
                        lblStatus.Content = String.Format("{0} : {1}", mediaPlayer.Position.ToString(@"mm\:ss"), mediaPlayer.NaturalDuration.TimeSpan.ToString(@"mm\:ss"));

                    }
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
            Console.WriteLine("test");
            Playlist playlist = listViewPlaylists.SelectedItem as Playlist;
            if (listViewPlaylists.SelectedItem == null)
            {
                playlist = Playlists[0];
            }
            List<MusicTrack> tracks = playlist.Tracks;

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



        private void ListViewPlaylist_ItemClick(object sender, MouseButtonEventArgs e)
        {

            ChangePlaylist(sender);
        }

        void btnAddToPlaylist_Click(Object sender, RoutedEventArgs e)
        {
            AddToSongToPlaylist(sender);
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
            CreatePlaylist();
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
                PlaySong();
            }
        }

        private void btnPrevious_Click(object sender, RoutedEventArgs e)
        {
            PreviousSong();
        }

        private void btnPlay_Click(object sender, RoutedEventArgs e)
        {
            PlaySong();

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

        private void TextBlock_SourceUpdated(object sender, DataTransferEventArgs e)
        {

        }
    }
}
