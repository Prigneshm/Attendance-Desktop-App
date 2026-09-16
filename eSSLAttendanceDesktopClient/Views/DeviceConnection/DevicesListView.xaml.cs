using eSSLAttendanceDesktopClient.Domain;
using eSSLAttendanceDesktopClient.Infrastructure;
using eSSLAttendanceDesktopClient.Infrastructure.IService;
using eSSLAttendanceDesktopClient.Service;
using eSSLAttendanceDesktopClient.Utility;
using eSSLAttendanceDesktopClient.Views.Employee;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace eSSLAttendanceDesktopClient.Views.DeviceConnection
{
    /// <summary>
    /// Interaction logic for DevicesListView.xaml
    /// </summary>
    public partial class DevicesListView : UserControl
    {
        #region [ Objects ]
        private readonly IDeviceService _deviceService;
        private readonly IAttendanceService _attendanceService;
        private readonly IAttendanceLogService _attendanceLogService;
        List<Domain.Device> mDevices;
        private DeviceLister deviceLister;
        private bool isWindowOpened = false;
        public event EventHandler isLoaderRefreshEvent;
        #endregion

        #region [ CTOR ]
        public DevicesListView()
        {
            try
            {
                InitializeComponent();
                _deviceService = new DeviceService();
                mDevices = new List<Domain.Device>();
                deviceLister = new DeviceLister();
                GetDevices();                
                _attendanceLogService = new AttendanceLogService();
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
                //SnackbarService.ShowError(ex.Message);
                SnackbarService.DisplayErrorMessage(ex.StackTrace);
            }
        }
        #endregion

        #region [ Methods ]
        private void ShowLoader(Visibility isLoading)
        {
            isLoaderRefreshEvent?.Invoke(isLoading, EventArgs.Empty);
        }

        private void GetDevices()
        {
            try
            {
                BackgroundWorker worker = new BackgroundWorker();
                worker.DoWork += (s, e) =>
                {
                    ShowLoader(Visibility.Visible);
                };
                var mLister = _deviceService.GetAll(deviceLister);
                if (mLister != null)
                {
                    this.deviceLister = mLister;
                    lstDevices.ItemsSource = null;


                    lblCurrentPageValue.Content = this.deviceLister.Pagination.CurrentPage + " / " + this.deviceLister.Pagination.TotalPage;
                    var totalEntries = this.deviceLister.Pagination.Skip + this.deviceLister.Pagination.Take;

                    if (totalEntries > this.deviceLister.Pagination.TotalRecord)
                        totalEntries = this.deviceLister.Pagination.TotalRecord;

                    lblShowingRecords.Content = $"Showing {this.deviceLister.Pagination.Skip + 1} to {totalEntries} of {this.deviceLister.Pagination.TotalRecord} entries";

                    if (deviceLister.List != null && deviceLister.List.Count > 0)
                    {
                        lblRecordNotfound.Visibility = Visibility.Hidden;
                        lstDevices.ItemsSource = deviceLister.List.ToList();
                    }
                    else
                    {
                        lblRecordNotfound.Visibility = Visibility.Visible;
                    }
                }
                ShowLoader(Visibility.Hidden);
            }
            catch (Exception ex)
            {
                ShowLoader(Visibility.Hidden);
                SnackbarService.DisplayErrorMessage(ex.Message);
            }
        }

        private void SetFilter(Domain.Device mdevice)
        {
            this.deviceLister.SearchCriteria = new Domain.Device();

            if (mdevice.IPAddress.IsNotNullOrEmpty() && mdevice.IPAddress.Length >= 3)
                this.deviceLister.SearchCriteria.IPAddress = mdevice.IPAddress;
            else
                this.deviceLister.SearchCriteria.IPAddress = null;

            if (mdevice.Name.IsNotNullOrEmpty() && mdevice.Name.Length >= 3)
                this.deviceLister.SearchCriteria.Name = mdevice.Name;
            else
                this.deviceLister.SearchCriteria.Name = null;

            if (mdevice.Port > 0)
                this.deviceLister.SearchCriteria.Port = mdevice.Port;
            else
                this.deviceLister.SearchCriteria.Port = 0;

            this.deviceLister.Pagination.TotalRecord = default(int);
            this.deviceLister.Pagination.Skip = default(int);
        }


        private void SetFilter()
        {
            try
            {
                this.deviceLister.SearchCriteria = new Domain.Device();

                if (txtIpAddress.Text.IsNotNullOrEmpty() && txtIpAddress.Text.Length >= 3)
                    this.deviceLister.SearchCriteria.IPAddress = txtIpAddress.Text;
                else
                    this.deviceLister.SearchCriteria.IPAddress = null;

                if (txtDeviceName.Text.IsNotNullOrEmpty() && txtDeviceName.Text.Length >= 3)
                    this.deviceLister.SearchCriteria.Name = txtDeviceName.Text;
                else
                    this.deviceLister.SearchCriteria.Name = null;

                if (txtPort.Text.IsNotNullOrEmpty())
                {
                    int intValue = int.Parse(txtPort.Text);
                    this.deviceLister.SearchCriteria.Port = intValue;
                }
                else
                    this.deviceLister.SearchCriteria.Port = 0;

                this.deviceLister.Pagination.TotalRecord = default(int);
                this.deviceLister.Pagination.Skip = default(int);
            }
            catch (Exception ex)
            {
                SnackbarService.DisplayErrorMessage(ex.Message);
            }
        }

        #endregion

        #region [ Events ]
        private void bdrAddDevice_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (!isWindowOpened)
                {
                    isWindowOpened = true;
                    var addDevice = new AddDeviceWindow();
                    addDevice.Closed += (s, args) =>
                    {
                        isWindowOpened = false;
                        GetDevices();
                    };
                    addDevice.Show();
                }
            }
            catch (Exception ex)
            {
                SnackbarService.DisplayErrorMessage(ex.Message);
            }
        }

        private void bdrReset_MouseDown(object sender, MouseButtonEventArgs e)
        {
            deviceLister.SearchCriteria = null;
            GetDevices();
        }

        private void BtnEdit_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is Image btnImg && btnImg.DataContext is Domain.Device device)
            {
                var addDeviceWindowa = new AddDeviceWindow(device);
                addDeviceWindowa.OpenAddDeviceWindow += (s1, e1) =>
                {
                    GetDevices();
                };
                addDeviceWindowa.Show();
            }
        }

        private void BtnDelete_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (sender is Image btnImg && btnImg.DataContext is Domain.Device device)
                {
                    var alertPopup = new AlertPopup();
                    alertPopup.refreshEvent += (s1, e1) =>
                    {
                        if (s1 is bool isYes && isYes == true)
                        {
                            _deviceService.Delete(device.Id);
                            GetDevices();
                        }
                    };
                    alertPopup.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                SnackbarService.DisplayErrorMessage(ex.Message);
            }
        }

        private async void BrdConnectDevice_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                ShowLoader(Visibility.Visible);

                var mDevice = new Domain.Device();

                // Extract device info from UI thread
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    if (sender is FrameworkElement frameworkElement &&
                        frameworkElement.DataContext is Domain.Device rowData)
                    {
                        mDevice.Id = rowData.Id;
                        mDevice.IPAddress = rowData.IPAddress;
                        mDevice.Port = rowData.Port;
                    }
                });

                if (string.IsNullOrWhiteSpace(mDevice.IPAddress) || mDevice.Port <= 0)
                {
                    SnackbarService.DisplayWarningMessage("IP Address or Port is not supplied!");
                    return;
                }

                await Task.Run(() =>
                {
                    try
                    {
                        var mAttendance = _attendanceService.GetAttendanceLogs(mDevice);

                        if (mAttendance?.Count > 0)
                        {
                            _attendanceLogService.ProcessLog(mDevice, mAttendance);
                            SnackbarService.ShowSuccess("Attendance log has been sync successfully!");
                        }
                        else
                        {
                            SnackbarService.ShowError("Something went wrong while sync log!");
                        }
                    }
                    catch (Exception ex)
                    {
                        SnackbarService.ShowError(ex.Message);
                    }
                });
            }
            catch (Exception ex)
            {
                SnackbarService.DisplayErrorMessage(ex.Message);
            }
            finally
            {
                ShowLoader(Visibility.Hidden);
            }
        }

        private void btnPrevious_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (this.deviceLister.Pagination.Skip >= this.deviceLister.Pagination.PageSize)
            {
                this.deviceLister.Pagination.Skip = this.deviceLister.Pagination.Skip - this.deviceLister.Pagination.PageSize;
            }

            GetDevices();
        }

        private void btnNext_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (this.deviceLister.Pagination.TotalPage != this.deviceLister.Pagination.CurrentPage)
            {
                this.deviceLister.Pagination.Skip = this.deviceLister.Pagination.Skip + this.deviceLister.Pagination.PageSize;
                this.deviceLister.Pagination.PageSize = this.deviceLister.Pagination.PageSize;
            }

            GetDevices();
        }

        private void bdrSearch_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //SetFilter();
            //GetDevices();
            if (!isWindowOpened)
            {
                isWindowOpened = true;
                var deviceFilterWindow = new DeviceFilterWindow();
                deviceFilterWindow.refreshEvent += (s1, e1) =>
                {
                    isWindowOpened = false;
                    if (s1 is Domain.Device device)
                    {
                        SetFilter(device);
                        GetDevices();
                    }
                };
                deviceFilterWindow.ShowDialog();
            }
            #endregion
        }

        private void txtFilterEmployeeId_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void DeviceNameFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox txtDeviceName)
            {
                SetFilter();
                GetDevices();
            }

        }

        private void IPAddressFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox txtIPAddress)
            {
                SetFilter();
                GetDevices();
            }
        }

        private void PortFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox txtPort)
            {
                SetFilter();
                GetDevices();
            }
        }

        private void StatusFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox txtStatus)
            {
                SetFilter();
                GetDevices();
            }
        }

        private void NumericTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Allow only digits
            e.Handled = !IsTextNumeric(e.Text);
        }

        private static bool IsTextNumeric(string text)
        {
            return text.All(char.IsDigit);
        }

        // Optional: prevent spacebar, etc.
        private void NumericTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            // Disallow space
            if (e.Key == Key.Space)
                e.Handled = true;
        }

        private void NumericTextBox_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(string)))
            {
                string text = (string)e.DataObject.GetData(typeof(string));
                if (!IsTextNumeric(text))
                {
                    e.CancelCommand();
                }
            }
            else
            {
                e.CancelCommand();
            }
        }

        private void cmbPageNumbers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
