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
using System.Windows.Shapes;

namespace eSSLAttendanceDesktopClient.Views.Employee
{
    /// <summary>
    /// Interaction logic for AlertPopup.xaml
    /// </summary>
    public partial class AlertPopup : Window
    {
        #region [Objects]
        public event EventHandler refreshEvent;
        public bool _islogout = false;
        #endregion

        #region [CTOR]
        public AlertPopup(bool isLogout = false)
        {
            InitializeComponent();
            _islogout = isLogout;
            if (!_islogout)
            {
                TitleMessage.Content = "Are you sure want to delete this record.";
            }
            else
            {
                TitleMessage.Content = "Are you sure want to logout.";
            }
        }
        #endregion

        #region [Events]
        private void BtnYes_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            refreshEvent?.Invoke(true, EventArgs.Empty);
        }

        private void BtnNo_Clicked(object sender, RoutedEventArgs e)
        {
            this.Close();
            refreshEvent?.Invoke(false, EventArgs.Empty);
        }

        private void imgClose_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        } 
        #endregion
    }
}
