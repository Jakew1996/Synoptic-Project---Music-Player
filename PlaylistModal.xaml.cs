using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

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
        }

        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            List<Playlist> playlist = new List<Playlist>();
            playlist.Add(new Playlist()
            {
                Name = TextBox.TextProperty.ToString()
            });


            Console.WriteLine(playlist);
            //mainWindow.listViewPlaylists.ItemsSource;
            this.Close();
        }
    }
}
