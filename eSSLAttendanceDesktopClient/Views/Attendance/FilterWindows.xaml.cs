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
using eSSLAttendanceDesktopClient.Domain;
using eSSLAttendanceDesktopClient.Infrastructure.IService;
using eSSLAttendanceDesktopClient.Model;
using eSSLAttendanceDesktopClient.Service;
using eSSLAttendanceDesktopClient.Utility;

namespace eSSLAttendanceDesktopClient.Views.Attendance
{
    /// <summary>
    /// Interaction logic for FilterWindows.xaml
    /// </summary>
    public partial class FilterWindows : Window
    {
        public event EventHandler refreshEvent;
        public AttendanceLog mAttendanceFilter;
        List<Domain.Designation> mDesignations;
        List<Domain.Department> mDepartments;
        private readonly Infrastructure.IService.IDesignationService designationService;
        private readonly Infrastructure.IService.IDepartmentService departmentService;

        private string _departmentName = string.Empty;
        private string _designationName = string.Empty;
        public FilterWindows()
        {
            InitializeComponent();
            mAttendanceFilter = new AttendanceLog();
            mDesignations = new List<Designation>();
            designationService = new DesignationService();
            departmentService = new DepartmentService();
            GetIsActiveDepartment();
            GetAllDesignations();
        }

        private void txtSearchByName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                mAttendanceFilter.EmployeeName = txtSearchByName.Text;
                this.Close();
                refreshEvent?.Invoke(mAttendanceFilter, EventArgs.Empty);
            }
        }

        private void bdrSearch_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                mAttendanceFilter.EmployeeName = txtSearchByName.Text;
                mAttendanceFilter.Department = _departmentName;
                mAttendanceFilter.Designation = _designationName;

                if (dtFrom.SelectedDate.HasValue && dtFrom.SelectedDate.Value != DateTime.MinValue)
                    this.mAttendanceFilter.FromDate = dtFrom.SelectedDate.Value;

                if (dtTo.SelectedDate.HasValue && dtTo.SelectedDate.Value != DateTime.MinValue)
                    this.mAttendanceFilter.ToDate = dtTo.SelectedDate.Value;
                this.Close();
                refreshEvent?.Invoke(mAttendanceFilter, EventArgs.Empty);
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void bdrReset_MouseDown(object sender, MouseButtonEventArgs e)
        {
            txtSearchByName.Text = string.Empty;
            //txtSearchByDepartment.Text = string.Empty;
            //txtSearchByDesignation.Text = string.Empty;
            dtFrom.SelectedDate = null;
            dtTo.SelectedDate = null;
        }

        private void txtSearchByDepartment_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void txtSearchByDesignation_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void CmboxDepartment_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ComboBox cmbx)
            {
                if (e.AddedItems.Count > 0 && e.AddedItems[0] is Domain.Department department)
                {
                    int SelectedDartmentId = department.Id;
                    _departmentName = department.Name;
                    GetIsActiveDesignations(SelectedDartmentId);
                }
            }
        }

        private List<Designation> GetIsActiveDesignations(int deptId = 0)
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

        private void CmboxDesignation_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ComboBox cmbx)
            {
                if (e.AddedItems.Count > 0 && e.AddedItems[0] is Domain.Designation designation)
                {
                    int SelectedDartmentId = designation.Id;
                    _designationName = designation.Name;
                }
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            refreshEvent?.Invoke(mAttendanceFilter, EventArgs.Empty);
        }
    }
}
