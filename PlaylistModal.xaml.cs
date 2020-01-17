using System.Collections.Generic;
using System.Linq;
using System.Windows;

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

            mainWindow.listViewPlaylists.ItemsSource = newList;
            mainWindow.listViewPlaylists.Items.Refresh();
            System.Console.WriteLine(mainWindow.listViewPlaylists.ItemsSource);

            //Console.WriteLine(playlist);
            //mainWindow.listViewPlaylists.ItemsSource;
        }
    }
}
