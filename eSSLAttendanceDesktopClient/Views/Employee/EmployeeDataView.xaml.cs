using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using eSSLAttendanceDesktopClient.Converter;
using eSSLAttendanceDesktopClient.Domain;
using eSSLAttendanceDesktopClient.Infrastructure;
using eSSLAttendanceDesktopClient.Service;
using eSSLAttendanceDesktopClient.Utility;

namespace eSSLAttendanceDesktopClient.Views.Employee
{
    /// <summary>
    /// Interaction logic for EmployeeDataView.xaml
    /// </summary>
    public partial class EmployeeDataView : UserControl
    {
        #region [Objects]
        public event EventHandler isLoaderRefreshEvent;
        private readonly Infrastructure.IService.IEmployeeService _employeeService;
        private bool isWindowOpened = false;
        private EmployeeLister EmployeeLister;
        List<Domain.Department> mDepartments;
        List<Domain.Designation> mDesignations;
        private readonly Infrastructure.IService.IDepartmentService departmentService;
        private readonly Infrastructure.IService.IDesignationService designationService;
        private string _departmentName = string.Empty;
        private string _designationName = string.Empty;
        private int _currentPage = 1;
        private int _totalPages = 0;
     
        private int _pageSize = 10; // you can adjust this
        private bool _isPageSizeLoaded = false;
        #endregion

        #region [CTOR]
        public EmployeeDataView()
        {
            InitializeComponent();
            _employeeService = new EmployeeService();
            EmployeeLister = new EmployeeLister();
            departmentService = new DepartmentService();
            designationService = new DesignationService();
            mDepartments = new List<Department>();
            mDesignations = new List<Designation>();
            SetFilter();
            BindEmployees();
            GetIsActiveDepartment();
            GetAllDesignations();
        }
        #endregion

        private void OnLoaderRefreshEvent(object sender, EventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                // Update UI elements for loader visibility
                loaderGif.Visibility = (Visibility)sender;
            });
        }

        private void ShowLoader(Visibility isLoading)
        {
            Debug.WriteLine($"ShowLoader called with: {isLoading}");
            isLoaderRefreshEvent?.Invoke(isLoading, EventArgs.Empty);
        }

        private void BindEmployees()
        {
            try
            {
                BackgroundWorker bgWorker = new BackgroundWorker();

                bgWorker.DoWork += (sender, args) =>
                {
                    // ✅ Update pagination request
                    EmployeeLister.Pagination.CurrentPage = _currentPage;
                    EmployeeLister.Pagination.Take = _pageSize;
                    EmployeeLister.Pagination.Skip = (_currentPage - 1) * _pageSize;

                    var employeeList = _employeeService.GetAll(EmployeeLister);

                    if (employeeList != null)
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            EmployeeLister = employeeList;

                            // ✅ Compute total pages correctly
                            _totalPages = (int)Math.Ceiling(
                                (double)EmployeeLister.Pagination.TotalRecord / EmployeeLister.Pagination.Take
                            );

                            // ✅ Bind records
                            if (EmployeeLister.List != null && EmployeeLister.List.Count > 0)
                            {
                                lstEmployee.ItemsSource = EmployeeLister.List.ToList();
                                lblRecordNotfound.Visibility = Visibility.Hidden;
                            }
                            else
                            {
                                lstEmployee.ItemsSource = null;
                                lblRecordNotfound.Visibility = Visibility.Visible;
                            }

                            // ✅ Update labels
                            lblCurrentPageValue.Content = $"{_currentPage} / {_totalPages}";

                            int totalEntries = EmployeeLister.Pagination.Skip + EmployeeLister.Pagination.Take;
                            if (totalEntries > EmployeeLister.Pagination.TotalRecord)
                                totalEntries = EmployeeLister.Pagination.TotalRecord;

                            lblShowingRecords.Content =
                                $"Showing {EmployeeLister.Pagination.Skip + 1} to {totalEntries} of {EmployeeLister.Pagination.TotalRecord} entries";
                        });
                    }
                };

                bgWorker.RunWorkerCompleted += (sender, args) =>
                {
                    Application.Current.Dispatcher.Invoke(() => ShowLoader(Visibility.Hidden));
                };

                Application.Current.Dispatcher.Invoke(() => ShowLoader(Visibility.Visible));
                bgWorker.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                SnackbarService.DisplayErrorMessage(ex.Message);
            }
        }

        private ScrollViewer GetScrollViewer(DependencyObject depObj)
        {
            if (depObj is ScrollViewer)
            {
                return (ScrollViewer)depObj;
            }

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                ScrollViewer result = GetScrollViewer(child);
                if (result != null)
                {
                    return result;
                }
            }
            return null;
        }

        private void SetFilter()
        {
            this.EmployeeLister.SearchCriteria = new Domain.Employee();

            if (txtSearchByName.Text.IsNotNullOrEmpty() && txtSearchByName.Text.Length >= 3)
                this.EmployeeLister.SearchCriteria.Name = txtSearchByName.Text;
            else
                this.EmployeeLister.SearchCriteria.Name = null;

            if (txtSearchByDeviceUniqueId.Text.IsNotNullOrEmpty() && txtSearchByDeviceUniqueId.Text.Length >= 1)
                this.EmployeeLister.SearchCriteria.DeviceUniqueId = txtSearchByDeviceUniqueId.Text;
            else
                this.EmployeeLister.SearchCriteria.DeviceUniqueId = null;

            if (txtSearchByDevice.Text.IsNotNullOrEmpty() && txtSearchByDevice.Text.Length >= 1)
                this.EmployeeLister.SearchCriteria.Device = txtSearchByDevice.Text;
            else
                this.EmployeeLister.SearchCriteria.Device = null;

            if (txtSearchByContact.Text.IsNotNullOrEmpty() && txtSearchByContact.Text.Length >= 1)
                this.EmployeeLister.SearchCriteria.Contact = txtSearchByContact.Text;
            else
                this.EmployeeLister.SearchCriteria.Contact = null;


            if (_departmentName.IsNotNullOrEmpty() && _departmentName.Length >= 1)
                this.EmployeeLister.SearchCriteria.Department = _departmentName;
            else
                this.EmployeeLister.SearchCriteria.Department = null;

            if (_designationName.IsNotNullOrEmpty() && _designationName.Length >= 1)
                this.EmployeeLister.SearchCriteria.Designation = _designationName;
            else
                this.EmployeeLister.SearchCriteria.Designation = null;


            this.EmployeeLister.Pagination.TotalRecord = default(int);
            this.EmployeeLister.Pagination.Skip = default(int);
        }

        #region [Events]
        private void bdrAddEmployee_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!isWindowOpened)
            {
                isWindowOpened = true;
                var addEmployeeWindow = new AddEmployeeWindow();
                addEmployeeWindow.refreshEvent += (s1, e1) =>
                {
                    isWindowOpened = false;
                    BindEmployees();
                };
                addEmployeeWindow.ShowDialog();
                //addEmployeeWindow.Closed += (s, args) => isWindowOpened = false;
                //addEmployeeWindow.Show();
            }
        }

        private void BtnEdit_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (sender is Image btnImg && btnImg.DataContext is Domain.Employee employee)
                {
                    // Show the loader
                    ShowLoader(Visibility.Visible);

                    var editWindow = new AddEmployeeWindow(employee, true);
                    editWindow.refreshEvent += (s1, e1) =>
                    {
                        BindEmployees();
                    };

                    // Hide the loader after the operation is complete
                    ShowLoader(Visibility.Hidden);

                    editWindow.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                _ = ex.Message;
            }
        }

        private async void BtnDelete_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                await Task.Delay(100);
                if (sender is Image btnImg && btnImg.DataContext is Domain.Employee employee)
                {
                    var alertPopup = new AlertPopup();
                    alertPopup.refreshEvent += (s1, e1) =>
                    {
                        if (s1 is bool isYes && isYes == true)
                        {
                            _employeeService.Delete(employee.Id);
                            BindEmployees();
                        }
                    };
                    alertPopup.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                _ = ex.Message;
            }
        }

        private void bdrReset_MouseDown(object sender, MouseButtonEventArgs e)
        {
            txtSearchByDeviceUniqueId.Text = string.Empty;
            txtSearchByName.Text = string.Empty;
            txtSearchByDevice.Text = string.Empty;
            txtSearchByContact.Text = string.Empty;
            CmboxDepartment.SelectedValue = null;
            CmboxDesignation.SelectedValue = null;
            _departmentName = string.Empty;
            _designationName = string.Empty;

            this.EmployeeLister.SearchCriteria = null;
            BindEmployees();
        }

        private void lstEmployee_MouseWheel(object sender, MouseWheelEventArgs e)
        {

        }

        private void lstEmployee_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            try
            {
                //ListView listView = (ListView)sender;
                //ScrollViewer scrollViewer = GetScrollViewer(listView);

                //if (scrollViewer != null && scrollViewer.VerticalOffset + scrollViewer.ViewportHeight >= scrollViewer.ExtentHeight)
                //{
                //    if (emplister.Pagination.CurrentPage < emplister.Pagination.TotalPage)
                //    {
                //        emplister.Pagination.CurrentPage++;
                //        int skip = (emplister.Pagination.CurrentPage - 1) * emplister.Pagination.PageSize;
                //        GetData(skip);
                //    }

                //}

                //if (e.VerticalOffset == e.ExtentHeight - e.ViewportHeight)
                //{
                //    // Scrolled to the bottom, simulate the "Next" button click
                // if (emplister.Pagination.CurrentPage < emplister.Pagination.TotalPage)
                //    {
                //        emplister.Pagination.CurrentPage++;
                //        int skip = (emplister.Pagination.CurrentPage - 1) * emplister.Pagination.PageSize;
                //        GetData(skip);
                //    }
                //}
                //// Check if the ListBox is scrolled to the top
                //else if (e.VerticalOffset == 0)
                //{
                //    // Scrolled to the top, simulate the "Previous" button click
                //    if (emplister.Pagination.CurrentPage > 1)
                //    {
                //        emplister.Pagination.CurrentPage--;
                //        int skip = (emplister.Pagination.CurrentPage - 1) * emplister.Pagination.PageSize;
                //        GetData(skip);
                //    }
                //}
            }
            catch (Exception ex)
            {
                _ = ex.Message;
            }
        }

        private void txtSearchByDeviceUniqueId_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                //SetFilter();
                BindEmployees();
            }
        }

        private void btnPrevious_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (this.EmployeeLister.Pagination.Skip >= this.EmployeeLister.Pagination.PageSize)
            {
                this.EmployeeLister.Pagination.Skip = this.EmployeeLister.Pagination.Skip - this.EmployeeLister.Pagination.PageSize;
            }

            BindEmployees();
        }

        private void btnNext_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (this.EmployeeLister.Pagination.TotalPage != this.EmployeeLister.Pagination.CurrentPage)
            {
                this.EmployeeLister.Pagination.Skip = this.EmployeeLister.Pagination.Skip + this.EmployeeLister.Pagination.PageSize;
                this.EmployeeLister.Pagination.PageSize = this.EmployeeLister.Pagination.PageSize;
            }

            BindEmployees();
        }

        private void bdrSearch_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //SetFilter(null);
            //BindEmployees();

            if (!isWindowOpened)
            {
                isWindowOpened = true;
                var employeeFilterWindows = new EmployeeFilter();
                employeeFilterWindows.refreshEvent += (s1, e1) =>
                {
                    isWindowOpened = false;
                    if (s1 is Domain.Employee employee)
                    {
                        //SetFilter(employee);
                        BindEmployees();
                    }
                };
                employeeFilterWindows.ShowDialog();
            }
        }
        #endregion

        private void BrdConnectDevice_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        //private void SetFilter(Domain.Employee employee)
        //{
        //    this.EmployeeLister.SearchCriteria = new Domain.Employee();
        //    //this.AttendanceLogLister.SearchCriteria = attendanceLog;

        //    if (employee.DeviceUniqueId.IsNotNullOrEmpty() && employee.DeviceUniqueId != "0")
        //        this.EmployeeLister.SearchCriteria.DeviceUniqueId = employee.DeviceUniqueId;
        //    else
        //        this.EmployeeLister.SearchCriteria.Id = 0;

        //    if (employee.Name.IsNotNullOrEmpty() && employee.Name.Length >= 3)
        //        this.EmployeeLister.SearchCriteria.Name = employee.Name;
        //    else
        //        this.EmployeeLister.SearchCriteria.Name = null;

        //    if (employee.Designation.IsNotNullOrEmpty() && employee.Designation.Length >= 3)
        //        this.EmployeeLister.SearchCriteria.Designation = employee.Designation;
        //    else
        //        this.EmployeeLister.SearchCriteria.Designation = null;


        //    this.EmployeeLister.Pagination.TotalRecord = default(int);
        //}

        private void EmployeeIdFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox txtIPAddress)
            {
                SetFilter();
                BindEmployees();
            }
        }

        private void NameFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox txtIPAddress)
            {
                SetFilter();
                BindEmployees();
            }
        }

        private void DepartmentFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox txtIPAddress)
            {
                SetFilter();
                BindEmployees();
            }
        }

        private void DesignationFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox txtIPAddress)
            {
                SetFilter();
                BindEmployees();
            }
        }

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
                    BindEmployees();
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
                    BindEmployees();
                }
            }

        }

        private void txtSearchByDevice_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox txtIPAddress)
            {
                SetFilter();
                BindEmployees();
            }
        }

        private void txtSearchByContact_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox txtIPAddress)
            {
                SetFilter();
                BindEmployees();
            }
        }

        private void ImgDepartmentReset_MouseDown(object sender, MouseButtonEventArgs e)
        {

            CmboxDepartment.SelectedValue = null;
            _departmentName = string.Empty;
            this.EmployeeLister.SearchCriteria = null;

            if (txtSearchByDeviceUniqueId.Text.IsNullOrEmpty() &&
                txtSearchByName.Text.IsNullOrEmpty() &&
                _designationName.IsNullOrEmpty() &&
                txtSearchByDevice.Text.IsNullOrEmpty() &&
                txtSearchByDevice.Text.IsNullOrEmpty())
            {
                BindEmployees();
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
            this.EmployeeLister.SearchCriteria = null;
            if (txtSearchByDeviceUniqueId.Text.IsNullOrEmpty() &&
                txtSearchByName.Text.IsNullOrEmpty() &&
                _departmentName.IsNullOrEmpty() &&
                txtSearchByDevice.Text.IsNullOrEmpty() &&
                txtSearchByDevice.Text.IsNullOrEmpty())
            {
                BindEmployees();
            }
            else
            {
                SetFilter();    
            }

        }
        // Optional: prevent spacebar, etc.
        private void NumericTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            // Disallow space
            if (e.Key == Key.Space)
                e.Handled = true;
        }
        private static bool IsTextNumeric(string text)
        {
            return text.All(char.IsDigit);
        }
        private void NumericTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Allow only digits
            e.Handled = !IsTextNumeric(e.Text);
        }

        private void EntriesPerPageComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_isPageSizeLoaded)
                return;   // Ignore auto-trigger during form load

            if (EntriesPerPageComboBox.SelectedItem is ComboBoxItem item)
            {
                string selected = item.Content.ToString();

                if (selected == "All")
                {
                    // After first load we know total records
                    _pageSize = EmployeeLister?.Pagination.TotalRecord ?? 0;
                }
                else
                {
                    _pageSize = int.Parse(selected);
                }

                // Reset to first page whenever size changes
                _currentPage = 1;

                BindEmployees();
            }
        }

    }
}
