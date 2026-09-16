using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using eSSLAttendanceDesktopClient.Domain;
using eSSLAttendanceDesktopClient.Infrastructure.IService;
using eSSLAttendanceDesktopClient.Service;
using eSSLAttendanceDesktopClient.Utility;
using Xceed.Wpf.Toolkit;

namespace eSSLAttendanceDesktopClient.Views.Attendance
{
    /// <summary>
    /// Interaction logic for CreateAttendanceLogWindows.xaml
    /// </summary>
    public partial class CreateAttendanceLogWindows : Window, INotifyPropertyChanged
    {
        #region [Objects]
        private readonly Infrastructure.IService.IEmployeeService employeeService;
        private readonly Infrastructure.IService.IAttendanceLogService attendanceLogService;
        Domain.AttendanceLog mAttendanceLog;
        public event EventHandler refreshEvent;
        public event PropertyChangedEventHandler PropertyChanged;

        private bool _isEdit = false;
        List<Domain.Employee> employees;
        private DateTime? _checkIn;
        private DateTime? _checkOut;
        #endregion

        #region [CTOR]
        public CreateAttendanceLogWindows(AttendanceLog attendanceLog, bool isEdit)
        {
            InitializeComponent();
            employeeService = new EmployeeService();
            attendanceLogService = new AttendanceLogService();
            mAttendanceLog = attendanceLog;
            _isEdit = isEdit;
            employees = new List<Domain.Employee>();
            GetEmployee();
            FillAttendanceObjectToControl();
        }
        #endregion

        #region [Methods]
        private List<Domain.Employee> GetEmployee()
        {
            employees = employeeService.GetAll();
            if (employees != null && employees.Count > 0)
            {
                CmboxEmployeeId.ItemsSource = employees.ToList();
            }
            return employees;
        }

        private void FillAttendanceObjectToControl()
        {
            if (mAttendanceLog != null && mAttendanceLog.Id > 0)
            {
                List<Domain.Employee> employeeList = GetEmployee();
                CmboxEmployeeId.DataContext = employeeList;
                CmboxEmployeeId.DisplayMemberPath = "EmployeeName";

                var selectedDepartment = employees.FirstOrDefault(d => d.Name == mAttendanceLog.EmployeeName);
                if (selectedDepartment != null)
                    CmboxEmployeeId.SelectedItem = selectedDepartment;

                dateTimePickerCheckIn.Value = mAttendanceLog.CheckIn;
                dateTimePickerCheckOut.Value = mAttendanceLog.CheckOut;
            }
        }

        private Domain.AttendanceLog PrepareAttendanceObject()
        {
            if (mAttendanceLog == null || (mAttendanceLog != null && mAttendanceLog.Id == 0))
            {
                mAttendanceLog = new Domain.AttendanceLog();
                mAttendanceLog.FromDate = DateTime.Now;
            }

            if (CmboxEmployeeId.SelectedItem != null)
            {
                if (CmboxEmployeeId.SelectedItem is Domain.Employee employee)
                {
                    mAttendanceLog.EmployeeId = employee.Id;
                }
            }
            mAttendanceLog.CheckIn = _checkIn;
            mAttendanceLog.CheckOut = _checkOut;

            return mAttendanceLog;
        }
        #endregion

        #region [Events]
        private void CommonSelection_ChangedEvent(object sender, SelectionChangedEventArgs e)
        {

        }

        private void CmboxCheckInt_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void CmboxEmployeeId_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void bdrSave_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var attendanceLog = PrepareAttendanceObject();
            if (attendanceLog != null && attendanceLog.Id > 0)
            {
                attendanceLogService.Update(attendanceLog.Id, attendanceLog);
                SnackbarService.DisplaySuccessMessage("Log details have been modified successfully!");
            }
            else
            {
                mAttendanceLog = attendanceLogService.Create(attendanceLog);
                if (mAttendanceLog != null && mAttendanceLog.Id > 0)
                {
                    this.Close();
                    SnackbarService.DisplaySuccessMessage("Log details have been Save successfully!");
                }
            }
            this.Close();
            refreshEvent?.Invoke(true, EventArgs.Empty);
        }

        private void imgClose_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }
        #endregion

        private void dateTimePickerCheckOut_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (sender is DateTimePicker checkOutdate)
            {
                _checkOut = checkOutdate.Value; // Update your variable here
            }
        }

        private void dateTimePickerCheckIn_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (sender is DateTimePicker checkOutdate)
            {
                _checkIn = checkOutdate.Value; // Update your variable here
            }
        }
    }
}
