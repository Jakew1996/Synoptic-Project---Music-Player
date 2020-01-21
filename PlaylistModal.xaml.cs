using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace MusicApp
{
    /// <summary>
    /// Interaction logic for PlaylistModal.xaml
    /// </summary>
    public partial class PlaylistModal : Window
    {
        public PlaylistModal()
        {
            InitializeComponent();
            this.Topmost = true;
        }

        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {

            createPlaylist();
            
            this.Close();
        }

        void createPlaylist()
        {
            MainWindow mainWindow = new MainWindow();
            List<Playlist> playlist = new List<Playlist>();
            //List<Playlist> currentPlaylists = new List<Playlist>();

            playlist.Add(new Playlist()
            {
                Name = PlaylistName.Text,
                Tracks = { },
            });

            List<Playlist> currentPlaylists = mainWindow.listViewPlaylists.ItemsSource.Cast<Playlist>().ToList();

            var newList = playlist.Concat(currentPlaylists);



            mainWindow.listViewPlaylists.ItemsSource = null;
            mainWindow.listViewPlaylists.ItemsSource = newList;

            System.ComponentModel.ICollectionView view = CollectionViewSource.GetDefaultView(mainWindow.listViewPlaylists.ItemsSource);
            view.Refresh();

        }
    }
}
