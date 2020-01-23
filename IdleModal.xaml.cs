using System.Windows;

namespace MusicApp
{
    /// <summary>
    /// Interaction logic for IdleModal.xaml
    /// </summary>
    public partial class IdleModal : Window
    {
        public IdleModal()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
