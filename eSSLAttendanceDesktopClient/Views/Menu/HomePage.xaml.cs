using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using eSSLAttendanceDesktopClient.Utility;
using eSSLAttendanceDesktopClient.Views.Account;
using eSSLAttendanceDesktopClient.Views.Attendance;
using eSSLAttendanceDesktopClient.Views.DeviceConnection;
using eSSLAttendanceDesktopClient.Views.Employee;

namespace eSSLAttendanceDesktopClient.Views.Menu
{
    /// <summary>
    /// Interaction logic for HomePage.xaml
    /// </summary>
    public partial class HomePage : Page
    {
        #region [CTOR]
        public HomePage()
        {
            InitializeComponent();
            LoadDefaultView();
        }
        #endregion

        #region [Methods]
        private void LoadDefaultView()
        {
            bdrEmployee.Background = (Brush)Application.Current.Resources["AppButton2"];
            var employeeDetailView = new EmployeeDataView();
            grdViews.Children.Add(employeeDetailView);
        }

        #endregion

        #region [Events]
        private async void bdrMenu_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                grdViews.Children.Clear();
                if (sender is Border bdrMenu)
                {
                    if (bdrMenu.Uid == DesktopAppEnum.MenuEnum.Employee.ToString())
                    {
                        bdrEmployee.Background = (Brush)Application.Current.Resources["AppButton2"];
                        bdrDevice.Background = Brushes.Transparent;
                        bdrAttendanceLog.Background = Brushes.Transparent;

                        loaderView.Visibility = Visibility.Visible;
                        await Task.Delay(50);
                        var employeeDetailView = new EmployeeDataView();
                        employeeDetailView.isLoaderRefreshEvent += (s1, e1) =>
                        {
                            if (s1 is Visibility isloading)
                                loaderView.Visibility = isloading;
                        };
                        grdViews.Children.Add(employeeDetailView);
                        loaderView.Visibility = Visibility.Hidden;
                    }
                    else if ((bdrMenu.Uid == DesktopAppEnum.MenuEnum.Device.ToString()))
                    {
                        loaderView.Visibility = Visibility.Visible;
                        await Task.Delay(500);
                        bdrDevice.Background = (Brush)Application.Current.Resources["AppButton2"];
                        bdrEmployee.Background = Brushes.Transparent;
                        bdrAttendanceLog.Background = Brushes.Transparent;

                        grdViews.Children.Add(new DevicesListView());
                        loaderView.Visibility = Visibility.Hidden;
                    }
                    else if ((bdrMenu.Uid == DesktopAppEnum.MenuEnum.Attendance_Log.ToString()))
                    {
                        loaderView.Visibility = Visibility.Visible;
                        await Task.Delay(500);
                        bdrAttendanceLog.Background = (Brush)Application.Current.Resources["AppButton2"];
                        bdrDevice.Background = Brushes.Transparent;
                        bdrEmployee.Background = Brushes.Transparent;

                        grdViews.Children.Add(new AttendanceLogView());
                        loaderView.Visibility = Visibility.Hidden;
                    }
                    else
                    {
                        bdrEmployee.Background = Brushes.Transparent;
                        bdrDevice.Background = Brushes.Transparent;
                        grdViews.Children.Add(new DeviceConnectionView());
                    }
                }
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        private void ImgLogout_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var alertPopup = new AlertPopup(isLogout: true);
                alertPopup.refreshEvent += (s1, e1) =>
                {
                    if (s1 is bool isYes && isYes == true)
                    {
                        var loginPage = new LoginPage();
                        NavigationService.Navigate(loginPage);
                    }
                };
                alertPopup.ShowDialog();
            }
            catch (System.Exception ex)
            {
                _ = ex.Message;
            }
        }
        #endregion

        //private void OnMouseEnterHandler(object sender, MouseEventArgs e)
        //{
        //    if (sender is Border bdrMenu)
        //    {
        //        if (bdrMenu.Uid == DesktopAppEnum.MenuEnum.Employee.ToString())
        //        {
        //            bdrEmployee.Background = (Brush)Application.Current.Resources["AppButton2"];
        //            lblemploye.Visibility = Visibility.Visible;
        //        }
        //        else if ((bdrMenu.Uid == DesktopAppEnum.MenuEnum.Device.ToString()))
        //        {
        //            bdrDevice.Background = (Brush)Application.Current.Resources["AppButton2"];
        //            lblDevice.Visibility = Visibility.Visible;
        //        }
        //        else if ((bdrMenu.Uid == DesktopAppEnum.MenuEnum.Attendance_Log.ToString()))
        //        {
        //            bdrAttendanceLog.Background = (Brush)Application.Current.Resources["AppButton2"];
        //            lblAttendance.Visibility = Visibility.Visible;
        //        }
        //    }
        //}

        //private void OnMouseLeaveHandler(object sender, MouseEventArgs e)
        //{
        //    if (sender is Border bdrMenu)
        //    {
        //        if (bdrMenu.Uid == DesktopAppEnum.MenuEnum.Employee.ToString())
        //        {
        //            bdrEmployee.Background = Brushes.Transparent;
        //            lblemploye.Visibility = Visibility.Hidden;
        //        }
        //        else if ((bdrMenu.Uid == DesktopAppEnum.MenuEnum.Device.ToString()))
        //        {
        //            bdrDevice.Background = Brushes.Transparent;
        //            lblDevice.Visibility = Visibility.Hidden;
        //        }
        //        else if ((bdrMenu.Uid == DesktopAppEnum.MenuEnum.Attendance_Log.ToString()))
        //        {
        //            bdrAttendanceLog.Background = Brushes.Transparent;
        //            lblAttendance.Visibility = Visibility.Hidden;
        //        }
        //    }
        //}
    }
}
