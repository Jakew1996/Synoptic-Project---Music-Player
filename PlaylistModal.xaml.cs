using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
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

            CreatePlaylist();
            
            this.Close();
        }

        void CreatePlaylist()
        {
            MainWindow mainWindow = new MainWindow();
            //mainWindow.AddPlaylist();
            mainWindow.Playlists.Add(new Playlist()
            {
                Name = PlaylistName.Text,
                Tracks = { },
            });
            Console.WriteLine(PlaylistName.Text);
            mainWindow.listViewPlaylists.DataContext = mainWindow.Playlists;

            //List<Playlist> playlist = new List<Playlist>();

            //playlist.Add(new Playlist()
            //{
            //  Name = PlaylistName.Text,
            //Tracks = { },
            //});

            //mainWindow.Playlists.Add((Playlist)mainWindow.listViewPlaylists.SelectedItem);

            //List<Playlist> currentPlaylists = new List<Playlist>();


            //List<Playlist> currentPlaylists = mainWindow.listViewPlaylists.ItemsSource.Cast<Playlist>().ToList();

            //var newList = playlist.Concat(currentPlaylists);

            //mainWindow.listViewPlaylists.ItemsSource = newList;
            //mainWindow.listViewPlaylists.Items.Add(new Playlist { Name = "PlaylistName.Text" });
        }
    }
}
