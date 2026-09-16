using System.Windows.Controls;

namespace eSSLAttendanceDesktopClient.Views.Menu
{
    /// <summary>
    /// Interaction logic for ToastNotification.xaml
    /// </summary>
    public partial class ToastNotification : UserControl
    {
        public ToastNotification()
        {
            InitializeComponent();
        }
        public void SetMessage(string message)
        {
            ToastMessageTextBlock.Text = message;
        }
    }
}
