using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using eSSLAttendanceDesktopClient.Converter;
using eSSLAttendanceDesktopClient.Domain;
using eSSLAttendanceDesktopClient.Infrastructure;
using eSSLAttendanceDesktopClient.Infrastructure.IService;
using eSSLAttendanceDesktopClient.Service;
using eSSLAttendanceDesktopClient.Utility;
using eSSLAttendanceDesktopClient.Views.Employee;
using MaterialDesignThemes.Wpf;

namespace eSSLAttendanceDesktopClient.Views.Attendance
{
    /// <summary>
    /// Interaction logic for AttendanceLogView.xaml
    /// </summary>
    public partial class AttendanceLogView : UserControl
    {
        #region [Objects]
        private bool isWindowOpened = false;
        AttendanceLogLister AttendanceLogLister;
        List<AttendanceLog> mAttendanceLog;
        public event EventHandler isLoaderRefreshEvent;
        private readonly IAttendanceLogService _attendanceLogService;
        public SnackbarMessageQueue SnackbarMessageQueue { get; } = new SnackbarMessageQueue();
        AttendanceLog attendanceLog = null;
        List<Domain.Department> mDepartments;
        List<Domain.Designation> mDesignations;
        private readonly Infrastructure.IService.IDepartmentService departmentService;
        private readonly Infrastructure.IService.IDesignationService designationService;
        private string _departmentName = string.Empty;
        private string _designationName = string.Empty;
        private int _currentPage = 1;
        private int _totalPages = 0;
        private bool _isPageSizeLoaded = false;
        #endregion

        #region [CTOR]
        public AttendanceLogView()
        {
            InitializeComponent();

            AttendanceLogLister = new AttendanceLogLister();
            _attendanceLogService = new AttendanceLogService();
            departmentService = new DepartmentService();
            designationService = new DesignationService();
            attendanceLog = new AttendanceLog();

            mDepartments = new List<Department>();
            mDesignations = new List<Designation>();
            mAttendanceLog = new List<AttendanceLog>();

            DataContext = this;

            _currentPage = 1; // start at page 1

            GetIsActiveDepartment();
            GetAllDesignations();

            GetAttendanceLogs(); // only call this once — LoadPageNumbers will run inside
            Loaded += (s, e) => _isPageSizeLoaded = true;
        }

        #endregion

        #region [Methods]
        private List<Department> GetIsActiveDepartment()
        {
            try
            {
                mDepartments = departmentService.GetAllActiveDepartments();
                if (mDepartments != null)
                {
                    CmboxDepartment.ItemsSource = mDepartments.ToList();
                }
            }
            catch (Exception ex)
            {
                SnackbarService.DisplayErrorMessage(ex.Message);
            }
            return mDepartments.ToList();
        }

        private List<Designation> GetAllDesignations()
        {
            try
            {
                mDesignations = designationService.GetAllDesignations();
                if (mDesignations != null)
                {
                    CmboxDesignation.ItemsSource = mDesignations.ToList();
                }
            }
            catch (Exception ex)
            {
                SnackbarService.DisplayErrorMessage(ex.Message);
            }
            return mDesignations.ToList();
        }
        private void ShowLoader(Visibility isLoading)
        {
            isLoaderRefreshEvent?.Invoke(isLoading, EventArgs.Empty);
        }

        private BackgroundWorker bgWorker;

        private void GetAttendanceLogs()
        {
            try
            {
                ShowLoader(Visibility.Visible);

                bgWorker = new BackgroundWorker();
                bgWorker.DoWork += delegate
                {
                    // Pass pagination info correctly
                    AttendanceLogLister.Pagination.CurrentPage = _currentPage;

                    var mLister = _attendanceLogService.GetAll(AttendanceLogLister);
                    if (mLister != null)
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            this.AttendanceLogLister = mLister;

                            // Calculate total pages correctly
                            _totalPages = (int)Math.Ceiling(
                                (double)this.AttendanceLogLister.Pagination.TotalRecord /
                                this.AttendanceLogLister.Pagination.Take
                            );

                            //Bind records
                            if (AttendanceLogLister.List != null && AttendanceLogLister.List.Count > 0)
                            {
                                mAttendanceLog = AttendanceLogLister.List;
                                lstAttendanceLogs.ItemsSource = AttendanceLogLister.List.ToList();
                                lblRecordNotfound.Visibility = Visibility.Hidden;
                            }
                            else
                            {
                                lblRecordNotfound.Visibility = Visibility.Visible;
                                lstAttendanceLogs.ItemsSource = null;
                            }

                            // Update UI labels
                            lblCurrentPageValue.Content =
                                $"{this.AttendanceLogLister.Pagination.CurrentPage} / {_totalPages}";

                            var totalEntries = this.AttendanceLogLister.Pagination.Skip +
                                               this.AttendanceLogLister.Pagination.Take;

                            if (totalEntries > this.AttendanceLogLister.Pagination.TotalRecord)
                                totalEntries = this.AttendanceLogLister.Pagination.TotalRecord;

                            lblShowingRecords.Content =
                                $"Showing {this.AttendanceLogLister.Pagination.Skip + 1} to {totalEntries} of {this.AttendanceLogLister.Pagination.TotalRecord} entries";
                        });
                    }
                };

                bgWorker.RunWorkerCompleted += delegate { ShowLoader(Visibility.Hidden); };
                bgWorker.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                ShowLoader(Visibility.Hidden);
                SnackbarService.DisplayErrorMessage(ex.Message);
            }
        }
        private void SetFilter()
        {
            this.AttendanceLogLister.SearchCriteria = new Domain.AttendanceLog();

            if (txtSearchByName.Text.IsNotNullOrEmpty() && txtSearchByName.Text.Length >= 3)
                this.AttendanceLogLister.SearchCriteria.EmployeeName = txtSearchByName.Text;
            else
                this.AttendanceLogLister.SearchCriteria.EmployeeName = null;


            if (dtFrom.SelectedDate.HasValue && dtFrom.SelectedDate.Value != DateTime.MinValue)
                this.AttendanceLogLister.SearchCriteria.FromDate = dtFrom.SelectedDate.Value;
            else
                this.AttendanceLogLister.SearchCriteria.FromDate = DateTime.MinValue;

            if (toDate.SelectedDate.HasValue && toDate
                .SelectedDate.Value != DateTime.MinValue)
                this.AttendanceLogLister.SearchCriteria.ToDate = toDate.SelectedDate.Value;
            else
                this.AttendanceLogLister.SearchCriteria.ToDate = DateTime.MinValue;

            if (_departmentName.IsNotNullOrEmpty() && _departmentName.Length >= 1)
                this.AttendanceLogLister.SearchCriteria.Department = _departmentName;
            else
                this.AttendanceLogLister.SearchCriteria.Department = null;

            if (_designationName.IsNotNullOrEmpty() && _designationName.Length >= 1)
                this.AttendanceLogLister.SearchCriteria.Designation = _designationName;
            else
                this.AttendanceLogLister.SearchCriteria.Designation = null;

            if (txtSearchByStatus.Text.IsNotNullOrEmpty() && txtSearchByStatus.Text.Length >= 3)
                this.AttendanceLogLister.SearchCriteria.Status = txtSearchByStatus.Text;
            else
                this.AttendanceLogLister.SearchCriteria.Status = null;

            this.AttendanceLogLister.Pagination.TotalRecord = default(int);
        }

        private void SetFilter(AttendanceLog attendanceLog)
        {
            this.AttendanceLogLister.SearchCriteria = new Domain.AttendanceLog();
            //this.AttendanceLogLister.SearchCriteria = attendanceLog;

            if (attendanceLog.EmployeeName.IsNotNullOrEmpty() && attendanceLog.EmployeeName.Length >= 3)
                this.AttendanceLogLister.SearchCriteria.EmployeeName = attendanceLog.EmployeeName;
            else
                this.AttendanceLogLister.SearchCriteria.EmployeeName = null;

            if (attendanceLog.Department.IsNotNullOrEmpty() && attendanceLog.Department.Length >= 3)
                this.AttendanceLogLister.SearchCriteria.Department = attendanceLog.Department;
            else
                this.AttendanceLogLister.SearchCriteria.Department = null;

            if (attendanceLog.Designation.IsNotNullOrEmpty() && attendanceLog.Designation.Length >= 3)
                this.AttendanceLogLister.SearchCriteria.Designation = attendanceLog.Designation;
            else
                this.AttendanceLogLister.SearchCriteria.Designation = null;

            if (attendanceLog.FromDate != null && attendanceLog.FromDate != DateTime.MinValue)
                this.AttendanceLogLister.SearchCriteria.FromDate = attendanceLog.FromDate.Date;
            else
                this.AttendanceLogLister.SearchCriteria.FromDate = DateTime.MinValue;

            if (attendanceLog.ToDate.HasValue && attendanceLog.ToDate.Value != DateTime.MinValue)
                this.AttendanceLogLister.SearchCriteria.ToDate = attendanceLog.ToDate.Value;
            else
                this.AttendanceLogLister.SearchCriteria.ToDate = DateTime.MinValue;

            this.AttendanceLogLister.Pagination.TotalRecord = default(int);
        }
        #endregion

        #region [Events]
        private void bdrAddAttendance_MouseDown(object sender, MouseButtonEventArgs e)
        {
            #region [ Old code ]
            //if (!isWindowOpened)
            //{
            //isWindowOpened = true;
            //var addAttendanceLogWindow = new CreateAttendanceLogWindows();
            //addAttendanceLogWindow.refreshEvent += (s1, e1) =>
            //{
            //};
            //addAttendanceLogWindow.ShowDialog();
            //addAttendanceLogWindow.Closed += (s, args) => isWindowOpened = false;
            //addAttendanceLogWindow.Show();
            //} 
            #endregion

            ExportToCsvAsync();
        }

        private void ExportToCsvAsync()
        {
            try
            {
                if (mAttendanceLog == null || mAttendanceLog.Count == 0)
                    return;

                string folderPath = @"D:\AttendanceReport";

                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                // Define CSV header manually (for correct order)
                var sb = new StringBuilder();
                sb.AppendLine("Department,Designation,Employee,Check-In,Check-Out,Working Hours,Overtime,Status");

                // Add each record row
                foreach (var record in mAttendanceLog)
                {
                    string checkInStr = record.CheckIn?.ToString("yyyy-MM-dd HH:mm") ?? "-";
                    string checkOutStr = record.CheckOut?.ToString("yyyy-MM-dd HH:mm") ?? "-";

                    string workingHours = "-";
                    if (record.CheckIn.HasValue && record.CheckOut.HasValue)
                    {
                        TimeSpan duration = record.CheckOut.Value - record.CheckIn.Value;
                        workingHours = $"{(int)duration.TotalHours:D2}:{duration.Minutes:D2}:{duration.Seconds:D2}";
                    }

                    string overtime = record.Overtime == "00:00:00" ? "-" : record.Overtime;

                    // Append CSV row
                    sb.AppendLine($"{record.Department},{record.Designation},{record.EmployeeName},'{checkInStr},'{checkOutStr},{workingHours},{overtime},{record.Status}");
                }

                // Save to Documents folder
                string fileName = $"AttendanceReport_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
                string filePath = Path.Combine(folderPath, fileName);

                File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8);

                if (File.Exists(filePath) && new FileInfo(filePath).Length > 0)
                {

                    SnackbarService.DisplaySuccessMessage($"CSV file exported successfully! Saved to: {filePath}");
                }
            }
            catch (Exception ex)
            {
                SnackbarService.DisplayErrorMessage(ex.Message);
            }
        }


        private void BtnEdit_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (sender is Border bdrEditButton && bdrEditButton.DataContext is Domain.AttendanceLog mAttendanceLog)
                {
                    var editWindow = new CreateAttendanceLogWindows(mAttendanceLog, true);
                    editWindow.refreshEvent += (s1, e1) =>
                    {
                        GetAttendanceLogs();
                    };
                    editWindow.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                SnackbarService.DisplayErrorMessage(ex.Message);
            }
        }

        private async void BtnDelete_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                await Task.Delay(100);
                if (sender is Border bdrDeleteButton && bdrDeleteButton.DataContext is Domain.AttendanceLog attendanceLog)
                {
                    var alertPopup = new AlertPopup();
                    alertPopup.refreshEvent += (s1, e1) =>
                    {
                        if (s1 is bool isYes && isYes == true)
                        {
                            _attendanceLogService.Delete(attendanceLog.Id);
                            GetAttendanceLogs();
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

        private void btnPrevious_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (this.AttendanceLogLister.Pagination.Skip >= this.AttendanceLogLister.Pagination.PageSize)
            {
                this.AttendanceLogLister.Pagination.Skip = this.AttendanceLogLister.Pagination.Skip - this.AttendanceLogLister.Pagination.PageSize;
            }

            GetAttendanceLogs();
        }

        private void btnNext_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (this.AttendanceLogLister.Pagination.TotalPage != this.AttendanceLogLister.Pagination.CurrentPage)
            {
                this.AttendanceLogLister.Pagination.Skip = this.AttendanceLogLister.Pagination.Skip + this.AttendanceLogLister.Pagination.PageSize;
                this.AttendanceLogLister.Pagination.PageSize = this.AttendanceLogLister.Pagination.PageSize;
            }

            GetAttendanceLogs();
        }

        private void bdrReset_MouseDown(object sender, MouseButtonEventArgs e)
        {
            txtSearchByName.Text = string.Empty;
            dtFrom.SelectedDate = null;
            toDate.SelectedDate = null;
            CmboxDepartment.SelectedValue = null;
            CmboxDesignation.SelectedValue = null;
            _departmentName = string.Empty;
            _designationName = string.Empty;
            txtSearchByStatus.Text = string.Empty;
            this.AttendanceLogLister.SearchCriteria = null;

            GetAttendanceLogs();
        }

        private void bdrSearch_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //SetFilter();
            //GetAttendanceLogs();
            if (!isWindowOpened)
            {
                isWindowOpened = true;
                var addEmployeeWindow = new FilterWindows();
                addEmployeeWindow.refreshEvent += (s1, e1) =>
                {
                    isWindowOpened = false;
                    if (s1 is AttendanceLog attendance)
                    {
                        SetFilter(attendance);
                        GetAttendanceLogs();
                    }
                };
                addEmployeeWindow.ShowDialog();
                //addEmployeeWindow.Closed += (s, args) => isWindowOpened = false;
                //addEmployeeWindow.Show();
            }
        }

        private void txtSearchByName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SetFilter(null);
                GetAttendanceLogs();
            }
        }
        #endregion

        private void Grid_QueryContinueDrag(object sender, QueryContinueDragEventArgs e)
        {

        }

        private void GridViewColumn_GotStylusCapture(object sender, StylusEventArgs e)
        {

        }

        private void lstAttendanceLogs_Scroll(object sender, System.Windows.Controls.Primitives.ScrollEventArgs e)
        {

        }

        private void EmployeeNameFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox txtEmployeeName)
            {
                SetFilter();
                GetAttendanceLogs();
            }
        }

        private void CheckInNameFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox checkInName)
            {
                SetFilter();
                GetAttendanceLogs();
            }
        }

        private void CheckOutNameFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox checkOut)
            {
                SetFilter();
                GetAttendanceLogs();
            }
        }

        private void CmboxDepartment_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ComboBox cmbx)
            {
                var department = cmbx.SelectedItem as Domain.Department;

                // Check if department is null — i.e., nothing selected
                var converter = new NullToVisibilityConverter();
                var visibility = (Visibility)converter.Convert(department, typeof(Visibility), null, CultureInfo.CurrentCulture);

                if (visibility == Visibility.Visible)
                {
                    txtSelectedDepartment.Visibility = visibility;
                }
                else
                {
                    txtSelectedDepartment.Visibility = visibility;
                    _departmentName = department?.Name;
                    SetFilter();
                    GetAttendanceLogs();
                }
            }
        }

        private void CmboxDesignation_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ComboBox cmbx)
            {
                var designation = cmbx.SelectedItem as Domain.Designation;

                // Check if department is null — i.e., nothing selected
                var converter = new NullToVisibilityConverter();
                var visibility = (Visibility)converter.Convert(designation, typeof(Visibility), null, CultureInfo.CurrentCulture);

                if (visibility == Visibility.Visible)
                {
                    txtSelectedDesignation.Visibility = visibility;
                }
                else
                {
                    txtSelectedDesignation.Visibility = visibility;
                    _designationName = designation?.Name;
                    SetFilter();
                    GetAttendanceLogs();
                }
            }
        }

        private void txtSearchByStatus_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox txtStatus)
            {
                SetFilter();
                GetAttendanceLogs();
            }
        }

        private void toDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            SetFilter();
            GetAttendanceLogs();
        }

        private void dtFrom_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            SetFilter();
            GetAttendanceLogs();
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

        private void ImgDepartmentReset_MouseDown(object sender, MouseButtonEventArgs e)
        {
            CmboxDepartment.SelectedValue = null;
            _departmentName = string.Empty;
            this.AttendanceLogLister.SearchCriteria = null;
            if (txtSearchByName.Text.IsNullOrEmpty() &&
                _designationName.IsNullOrEmpty() &&
                dtFrom.SelectedDate == null &&
                toDate.SelectedDate == null &&
                txtSearchByStatus.Text.IsNullOrEmpty())
            {
                GetAttendanceLogs();
            }
            else
            {
                SetFilter();
            }
        }

        private void ImgDesignationReset_MouseDown(object sender, MouseButtonEventArgs e)
        {
            CmboxDesignation.SelectedValue = null;
            _designationName = string.Empty;
            this.AttendanceLogLister.SearchCriteria = null;

            if (txtSearchByName.Text.IsNullOrEmpty() &&
                _departmentName.IsNullOrEmpty() &&
                dtFrom.SelectedDate == null &&
                toDate.SelectedDate == null &&
                txtSearchByStatus.Text.IsNullOrEmpty())
            {
                GetAttendanceLogs();
            }
            else
            {
                SetFilter();
            }
        }

        private void ImgCheckInDateReset_MouseDown(object sender, MouseButtonEventArgs e)
        {
            dtFrom.SelectedDate = null;
            if (txtSearchByName.Text.IsNullOrEmpty() &&
                _designationName.IsNullOrEmpty() &&
                toDate.SelectedDate == null &&
                txtSearchByStatus.Text.IsNullOrEmpty())
            {
                GetAttendanceLogs();
            }
            else
            {
                SetFilter();
            }
        }

        private void ImgCheckOutDateReset_MouseDown(object sender, MouseButtonEventArgs e)
        {
            toDate.SelectedDate = null;
            if (txtSearchByName.Text.IsNullOrEmpty() &&
                _designationName.IsNullOrEmpty() &&
                dtFrom.SelectedDate == null &&
                txtSearchByStatus.Text.IsNullOrEmpty())
            {
                GetAttendanceLogs();
            }
            else
            {
                SetFilter();
            }
        }

        private void EntriesPerPageComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_isPageSizeLoaded)
                return;   // ignore initial firing

            if (AttendanceLogLister == null)
                return;   // Prevent null crash

            if (EntriesPerPageComboBox.SelectedItem is ComboBoxItem item)
            {
                string selected = item.Content.ToString();

                // Handle "All"
                if (selected == "All")
                    AttendanceLogLister.Pagination.Take = AttendanceLogLister.Pagination.TotalRecord;
                else
                    AttendanceLogLister.Pagination.Take = int.Parse(selected);

                _currentPage = 1;
                GetAttendanceLogs();
            }
        }

    }
}
