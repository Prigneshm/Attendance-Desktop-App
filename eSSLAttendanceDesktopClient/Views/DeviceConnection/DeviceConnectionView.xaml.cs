using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace eSSLAttendanceDesktopClient.Views.DeviceConnection
{
    /// <summary>
    /// Interaction logic for DeviceConnectionView.xaml
    /// </summary>
    public partial class DeviceConnectionView : UserControl
    {
        public ObservableCollection<string> Items { get; set; }
        public DeviceConnectionView()
        {
            InitializeComponent();
            Items = new ObservableCollection<string>
            {
                "Item 1",
                "Item 2",
                "Item 3"
            };
        }

        private void bdrConnect_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void bdrConnect_MouseEnter(object sender, MouseEventArgs e)
        {
        }
    }
}
