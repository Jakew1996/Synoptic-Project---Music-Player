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
            string folderPath = "C:\\Users\\Public\\Music";
            List<MusicTrack> tracks = new List<MusicTrack>();

            foreach (string file in Directory.EnumerateFiles(folderPath, "*.mp3"))
            {
                var tfile = TagLib.File.Create(file);
                MusicTrack track = new MusicTrack();
                track.Name = tfile.Tag.Title;
                track.Artists = tfile.Tag.Performers;
                track.Album = tfile.Tag.Album;
                track.FileLocation = file;

                
                tracks.Add(new MusicTrack() { Name = track.Name, Artists = track.Artists, Album = track.Album, FileLocation = track.FileLocation });
                listViewData.ItemsSource = tracks;
            }
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(listViewData.ItemsSource);
            view.Filter = UserFilter;
        }

        private void listView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void listViewData_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void ListViewItem_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var baseobj = sender as FrameworkElement;
            var musicTrack = baseobj.DataContext as MusicTrack;
            mediaPlayer.Open(new Uri(musicTrack.FileLocation));
            //btnPlay_Click(musicTrack);

            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += timer_Tick;
            timer.Start();
        }

        void ListView_MouseDoubleClick(Object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                var baseobj = sender as FrameworkElement;
                var musicTrack = baseobj.DataContext as MusicTrack;
                mediaPlayer.Open(new Uri(musicTrack.FileLocation));

                DispatcherTimer timer = new DispatcherTimer();
                timer.Interval = TimeSpan.FromSeconds(1);
                timer.Tick += timer_Tick;
                timer.Start();
                mediaPlayer.Play();
            }

        }

        void timer_Tick(object sender, EventArgs e)
        {
            if (mediaPlayer.Source != null)
                if (mediaPlayer.NaturalDuration.HasTimeSpan)
                    lblStatus.Content = String.Format("{0} / {1}", mediaPlayer.Position.ToString(@"mm\:ss"), mediaPlayer.NaturalDuration.TimeSpan.ToString(@"mm\:ss"));
            else
                lblStatus.Content = "No file selected...";
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
    }
}
