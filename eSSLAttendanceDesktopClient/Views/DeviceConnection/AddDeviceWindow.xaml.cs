using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using eSSLAttendanceDesktopClient.Infrastructure;
using eSSLAttendanceDesktopClient.Infrastructure.IService;
using eSSLAttendanceDesktopClient.Service;
using eSSLAttendanceDesktopClient.Utility;

namespace eSSLAttendanceDesktopClient.Views.DeviceConnection
{
    /// <summary>
    /// Interaction logic for AddDeviceWindow.xaml
    /// </summary>
    public partial class AddDeviceWindow : Window
    {
        #region [Objects]
        public event EventHandler OpenAddDeviceWindow;
        private bool _isActive = false;
        public Domain.Device mDevice;
        private readonly IDeviceService _deviceService;
        #endregion

        #region [CTOR]
        public AddDeviceWindow(Domain.Device mDevice = null)
        {
            InitializeComponent();
            this.mDevice = mDevice;
            _deviceService = new DeviceService();
            if (mDevice != null && mDevice.Id > 0)
            {
                lblTitle.Content = "Edit Device";
                txtDeviceName.Text = mDevice.Name;
                txtIPAddress.Text = mDevice.IPAddress;
                txtPort.Text = mDevice.Port.ConvertToString();
                if (mDevice.IsActive)
                {
                    imgCheckBox.Source = new BitmapImage(new Uri("pack://application:,,,/Images/icon_black_check.png"));
                    _isActive = true;
                }
                else
                {
                    imgCheckBox.Source = new BitmapImage(new Uri("pack://application:,,,/Images/icon_black_uncheck.png"));
                    _isActive = false;
                }
            }
        }
        #endregion

        #region [Methods]
        private Domain.Device PrepareDeviceObject()
        {
            try
            {
                if (mDevice == null || (mDevice != null && mDevice.Id == 0))
                {
                    mDevice = new Domain.Device();
                    mDevice.FromDate = DateTime.Now;
                }

                mDevice.Name = txtDeviceName.Text;
                mDevice.IPAddress = txtIPAddress.Text;
                mDevice.Port = txtPort.Text.ConvertToInt32();
                mDevice.IsActive = _isActive;
            }
            catch (Exception ex)
            {
                SnackbarService.DisplayErrorMessage(ex.Message);
                return null;
            }
            return mDevice;
        }

        private bool ValidateControl()
        {
            bool result = false;

            if (txtIPAddress.Text.IsNullOrEmpty() && txtPort.Text.IsNullOrEmpty())
            {
                txtWarningName.Visibility = Visibility.Visible;
                txtWarningIP.Visibility = Visibility.Visible;
                txtWarningPort.Visibility = Visibility.Visible;
            }
            else if (txtWarningName.Text.IsNullOrEmpty())
            {
                txtWarningName.Visibility = Visibility.Visible;
            }
            else if (txtIPAddress.Text.IsNullOrEmpty())
            {
                txtWarningIP.Visibility = Visibility.Visible;
            }
            else if (txtPort.Text.IsNullOrEmpty())
            {
                txtWarningPort.Visibility = Visibility.Visible;
            }
            else
            {
                result = true;
            }

            return result;
        }

        private void SaveDevice()
        {
            try
            {
                var mDevice = PrepareDeviceObject();
                if (mDevice != null && mDevice.Id > 0)
                {
                    _deviceService.Update(mDevice.Id, mDevice);
                    this.Close();
                    SnackbarService.DisplaySuccessMessage("Device details have been modified successfully!");
                    OpenAddDeviceWindow?.Invoke(true, EventArgs.Empty);
                }
                else
                {
                    var efDevice = _deviceService.Create(mDevice);
                    if (efDevice != null && efDevice.Id > 0)
                    {
                        this.Close();
                        OpenAddDeviceWindow?.Invoke(true, EventArgs.Empty);
                        SnackbarService.DisplaySuccessMessage("Device details have been created successfully!");
                    }
                }
            }
            catch (Exception ex)
            {
                SnackbarService.DisplaySuccessMessage(ex.Message);
            }
        }
        #endregion

        #region [Events]
        private void stkActive_MouseEnter(object sender, MouseButtonEventArgs e)
        {
            if (imgCheckBox.Source.ToString().Contains("icon_black_uncheck.png"))
            {
                imgCheckBox.Source = new BitmapImage(new Uri("pack://application:,,,/Images/icon_black_check.png"));
                _isActive = true;
            }
            else
            {
                imgCheckBox.Source = new BitmapImage(new Uri("pack://application:,,,/Images/icon_black_uncheck.png"));
                _isActive = false;
            }
        }

        private void bdrSave_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (ValidateControl())
            {
                SaveDevice();
            }
        }

        private void imgClose_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }

        private void txtIPAddress_TextChanged(object sender, TextChangedEventArgs e)
        {
            txtWarningIP.Visibility = Visibility.Hidden;
        }

        private void txtDeviceName_TextChanged(object sender, TextChangedEventArgs e)
        {
            txtWarningName.Visibility = Visibility.Hidden;
        }

        private void txtPort_TextChanged(object sender, TextChangedEventArgs e)
        {
            txtWarningPort.Visibility = Visibility.Hidden;
        }

        private void txtKey_KeyDown(object sender, KeyEventArgs e)
        {
            if (sender is TextBox txtbox)
            {
                if (e.Key == Key.Enter)
                {
                    if (txtbox.Uid == "Name")
                    {
                        txtIPAddress.Focus();
                    }
                    else if (txtbox.Uid == "IP")
                    {
                        txtPort.Focus();
                    }
                    else if (txtbox.Uid == "Port")
                    {
                        SaveDevice();
                    }
                }
            }
        } 
        #endregion
    }
}
