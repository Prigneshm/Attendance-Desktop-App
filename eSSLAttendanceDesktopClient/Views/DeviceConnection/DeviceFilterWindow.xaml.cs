using System;
using System.Windows;
using System.Windows.Input;
using eSSLAttendanceDesktopClient.Infrastructure;

namespace eSSLAttendanceDesktopClient.Views.DeviceConnection
{
    /// <summary>
    /// Interaction logic for DeviceFilterWindow.xaml
    /// </summary>
    public partial class DeviceFilterWindow : Window
    {
        public Domain.Device mDevice;
        public event EventHandler refreshEvent;
        public DeviceFilterWindow()
        {
            InitializeComponent();
            mDevice = new Domain.Device();
        }

        private void txtSearchByName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                mDevice.Name = txtSearchByName.Text;
                this.Close();
                refreshEvent?.Invoke(mDevice, EventArgs.Empty);
            }
        }

        private void txtSearchByIp_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                mDevice.IPAddress = txtSearchByIp.Text;
                this.Close();
                refreshEvent?.Invoke(mDevice, EventArgs.Empty);
            }
        }

        private void txtSearchByPort_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                int myInt = Convert.ToInt32(txtSearchByPort.Text);
                mDevice.Port = myInt;
                this.Close();
                refreshEvent?.Invoke(mDevice, EventArgs.Empty);
            }
        }

        private void bdrSearch_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                mDevice.Name = txtSearchByName.Text;
                mDevice.IPAddress = txtSearchByIp.Text;
                if (txtSearchByPort.Text.IsNotNullOrEmpty())
                {
                    int myInt = Convert.ToInt32(txtSearchByPort.Text);
                    mDevice.Port = myInt;
                }

                this.Close();
                refreshEvent?.Invoke(mDevice, EventArgs.Empty);
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void bdrReset_MouseDown(object sender, MouseButtonEventArgs e)
        {
            txtSearchByName.Text = string.Empty;
            txtSearchByIp.Text = string.Empty;
            txtSearchByPort.Text = string.Empty;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            refreshEvent?.Invoke(mDevice, EventArgs.Empty);
        }
    }
}
