using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using eSSLAttendanceDesktopClient.Domain;
using eSSLAttendanceDesktopClient.Infrastructure;
using eSSLAttendanceDesktopClient.Service;
using eSSLAttendanceDesktopClient.Utility;

namespace eSSLAttendanceDesktopClient.Views.Employee
{
    /// <summary>
    /// Interaction logic for AddEmployeeWindow.xaml
    /// </summary>
    public partial class AddEmployeeWindow : Window
    {
        #region [ Objects ]
        private readonly Infrastructure.IService.IEmployeeService employeeService;
        private readonly Infrastructure.IService.IDepartmentService departmentService;
        private readonly Infrastructure.IService.IDeviceService deviceService;
        private readonly Infrastructure.IService.IDesignationService designationService;
        Domain.Employee mEmployee;
        List<Domain.Department> mDepartments;
        List<Domain.Designation> mDesignations;
        List<Domain.Device> mDevices;
        public event EventHandler refreshEvent;
        bool _isEdit = false;
        #endregion

        #region [ Constuctor ]
        public AddEmployeeWindow(Domain.Employee employee = null, bool isEdit = false)
        {
            try
            {
                InitializeComponent();
                employeeService = new EmployeeService();
                departmentService = new DepartmentService();
                designationService = new DesignationService();
                deviceService = new DeviceService();
                mDepartments = new List<Domain.Department>();
                mDesignations = new List<Domain.Designation>();
                mDevices = new List<Domain.Device>();
                _isEdit = isEdit;
                if (!_isEdit)
                    lblTitle.Content = "Add Employee";
                else
                    lblTitle.Content = "Edit Employee";

                this.mEmployee = employee;

                GetIsActiveDevices();
                GetIsActiveDepartment();

                if (this.mEmployee != null)
                    FillObjectToControl();
            }
            catch (Exception ex)
            {
                SnackbarService.DisplayErrorMessage(ex.Message);
            }
        }
        #endregion

        #region [ Methods ]
        private void FillObjectToControl()
        {
            try
            {
                if (this.mEmployee != null)
                {
                    txtEmployeeID.Text = mEmployee.DeviceUniqueId;
                    txtFirstName.Text = mEmployee.FirstName;
                    txtLastName.Text = mEmployee.LastName;
                    txtContact.Text = mEmployee.Contact;

                    List<Department> departmentList = GetIsActiveDepartment();
                    CmboxDepartment.DataContext = departmentList;
                    CmboxDepartment.DisplayMemberPath = "Name";

                    var selectedDepartment = departmentList.FirstOrDefault(d => d.Name == mEmployee.Department);
                    if (selectedDepartment != null)
                        CmboxDepartment.SelectedItem = selectedDepartment;

                    if (mDesignations != null)
                    {
                        CmboxDepartment.DataContext = mDesignations;
                        CmboxDepartment.DisplayMemberPath = "Name";

                        var selectedDesignation = mDesignations.FirstOrDefault(d => d.Name == mEmployee.Designation);
                        if (selectedDesignation != null)
                            CmboxDesignation.SelectedItem = selectedDesignation;
                    }

                    if (mDevices != null)
                    {
                        CmboxDevice.DataContext = mDevices;
                        CmboxDevice.DisplayMemberPath = "Name";

                        var selectedmDevice = mDevices.FirstOrDefault(d => d.Name == mEmployee.Device);
                        if (selectedmDevice != null)
                            CmboxDevice.SelectedItem = selectedmDevice;
                    }

                    workingHoursComboBox.Text = mEmployee.WorkingHours.ToString();

                    if (mEmployee.WorkingHours > default(int))
                    {
                        if (mEmployee.WorkingHours == 8)
                            workingHoursComboBox.Text = "8 hour";
                        else if (mEmployee.WorkingHours == 10)
                            workingHoursComboBox.Text = "10 hour";
                        else if (mEmployee.WorkingHours == 12)
                            workingHoursComboBox.Text = "12 hour";
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private List<Domain.Device> GetIsActiveDevices()
        {
            try
            {
                mDevices = deviceService.GetDeviceList();
                if (mDevices != null)
                {
                    CmboxDevice.ItemsSource = mDevices.ToList();
                }
            }
            catch (Exception ex)
            {
                SnackbarService.DisplayErrorMessage(ex.Message);
            }
            return mDevices;
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

        private List<Designation> GetIsActiveDesignations(int deptId)
        {
            try
            {
                mDesignations = designationService.GetAllActiveDesignations(deptId);
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

        private bool ValidateData()
        {
            bool isvalid = false;
            if (txtEmployeeID.Text.IsNullOrEmpty() &&
                CmboxDepartment.SelectedIndex == -1 &&
                CmboxDesignation.SelectedIndex == -1 &&
                CmboxDevice.SelectedIndex == -1 &&
                txtFirstName.Text.IsNullOrEmpty() &&
                txtContact.Text.IsNullOrEmpty() &&
                workingHoursComboBox.SelectedIndex == -1)
            {
                EreEmployeeId.Visibility = Visibility.Visible;
                EreFirstName.Visibility = Visibility.Visible;
                EreContact.Visibility = Visibility.Visible;
                EreDepartment.Visibility = Visibility.Visible;
                EreDesignation.Visibility = Visibility.Visible;
                EreDevice.Visibility = Visibility.Visible;
                EreWorkingHours.Visibility = Visibility.Visible;
            }
            else if (txtEmployeeID.Text.IsNullOrEmpty())
            {
                EreEmployeeId.Visibility = Visibility.Visible;
            }
            else if (CmboxDepartment.SelectedIndex == -1)
            {
                EreDepartment.Visibility = Visibility.Visible;
            }
            else if (CmboxDesignation.SelectedIndex == -1)
            {
                EreDesignation.Visibility = Visibility.Visible;
            }
            else if (CmboxDevice.SelectedIndex == -1)
            {
                EreDevice.Visibility = Visibility.Visible;
            }
            else if (txtFirstName.Text.IsNullOrEmpty())
            {
                EreFirstName.Visibility = Visibility.Visible;
            }
            else if (txtContact.Text.IsNullOrEmpty() && txtContact.Text.Length <= 10)
            {
                EreContact.Visibility = Visibility.Visible;
            }
            else if (workingHoursComboBox.SelectedIndex == -1)
            {
                EreWorkingHours.Visibility = Visibility.Visible;
            }
            else
            {
                isvalid = true;
            }
            return isvalid;
        }

        private Domain.Employee PrepareObject()
        {
            try
            {
                if (mEmployee == null || (mEmployee != null && mEmployee.Id == 0))
                {
                    mEmployee = new Domain.Employee();
                    mEmployee.FromDate = DateTime.Now;
                }
                mEmployee.DeviceUniqueId = txtEmployeeID.Text;
                mEmployee.FirstName = txtFirstName.Text;
                mEmployee.LastName = txtLastName.Text;
                mEmployee.Contact = txtContact.Text;

                if (CmboxDepartment.SelectedItem != null)
                {
                    if (CmboxDepartment.SelectedItem is Department department)
                    {
                        mEmployee.DepartmentId = department.Id;
                    }

                    //ComboBoxItem selectedItem = CmboxDepartment.SelectedItem as ComboBoxItem;
                    //mEmployee.Designation = selectedItem.Content.ToString();
                }

                if (CmboxDesignation.SelectedItem != null)
                {
                    if (CmboxDesignation.SelectedItem is Designation designation)
                    {
                        mEmployee.DesignationId = designation.Id;
                    }
                }

                if (CmboxDevice.SelectedItem != null)
                {
                    if (CmboxDevice.SelectedItem is Domain.Device device)
                    {
                        mEmployee.DeviceId = device.Id;
                    }
                }

                if (workingHoursComboBox.SelectedItem != null)
                {
                    if (workingHoursComboBox.SelectedIndex == 0)
                        mEmployee.WorkingHours = 8;
                    else if (workingHoursComboBox.SelectedIndex == 1)
                        mEmployee.WorkingHours = 10;
                    else
                        mEmployee.WorkingHours = 12;
                }
            }
            catch (Exception ex)
            {
                SnackbarService.DisplayErrorMessage(ex.Message);
            }

            return mEmployee;
        }
        #endregion

        #region [ Events ]
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            refreshEvent?.Invoke(true, EventArgs.Empty);
        }
        private void imgClose_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.Close();
            refreshEvent?.Invoke(true, EventArgs.Empty);
        }

        private void bdrSave_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (ValidateData())
                {
                    var employee = PrepareObject();
                    Domain.Employee efEmployee = null;
                    if (employee != null && employee.Id > 0)
                    {
                        employeeService.Update(employee.Id, employee);
                        SnackbarService.DisplaySuccessMessage("Employee details have been modified successfully!");
                    }
                    else
                    {
                        efEmployee = employeeService.Create(employee);
                        if (efEmployee != null && efEmployee.Id > 0)
                        {
                            SnackbarService.DisplaySuccessMessage("Employee details have been created successfully!");
                        }

                    }
                    this.Close();
                    refreshEvent?.Invoke(true, EventArgs.Empty);
                }
            }
            catch (Exception ex)
            {
                SnackbarService.DisplayErrorMessage(ex.Message);
            }
        }

        private void CmboxDepartment_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ComboBox cmbx)
            {
                if (e.AddedItems.Count > 0 && e.AddedItems[0] is Domain.Department department)
                {
                    int SelectedDartmentId = department.Id;
                    EreDepartment.Visibility = Visibility.Hidden;
                    GetIsActiveDesignations(SelectedDartmentId);
                }
            }
        }

        private void CommonText_ChangedEvent(object sender, TextChangedEventArgs e)
        {
            if (sender is System.Windows.Controls.TextBox textBox)
            {
                if (textBox.Uid == "EmpID")
                    EreEmployeeId.Visibility = Visibility.Hidden;
                else if (textBox.Uid == "FName")
                    EreFirstName.Visibility = Visibility.Hidden;
                else if (textBox.Uid == "ContactNo")
                    EreContact.Visibility = Visibility.Hidden;
            }
        }

        private void CommonSelection_ChangedEvent(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ComboBox comboBox)
            {
                if (comboBox.Uid == "DepartmentId")
                    EreDepartment.Visibility = Visibility.Hidden;
                else if (comboBox.Uid == "DesignationId")
                    EreDesignation.Visibility = Visibility.Hidden;
                else if (comboBox.Uid == "DeviceId")
                    EreDevice.Visibility = Visibility.Hidden;
                else if (comboBox.Uid == "WrokingHour")
                    EreWorkingHours.Visibility = Visibility.Hidden;
            }
        }
        
        #endregion
    }
}
