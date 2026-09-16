using System.Windows;
using eSSLAttendanceDesktopClient.Views.Account;

namespace eSSLAttendanceDesktopClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.Width = 1300;
            this.Height = 800;
            MainFrame.Navigate(new LoginPage());
        }
    }
}
