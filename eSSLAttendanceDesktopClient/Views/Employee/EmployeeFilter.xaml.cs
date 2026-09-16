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
using eSSLAttendanceDesktopClient.Model;
using eSSLAttendanceDesktopClient.Service;
using eSSLAttendanceDesktopClient.Utility;

namespace eSSLAttendanceDesktopClient.Views.Employee
{
    /// <summary>
    /// Interaction logic for EmployeeFilter.xaml
    /// </summary>
    public partial class EmployeeFilter : Window
    {
        public event EventHandler refreshEvent;
        public Domain.Employee mEmployee;
        List<Domain.Designation> mDesignations;
        private string _designationName = string.Empty;
        private readonly Infrastructure.IService.IDesignationService designationService;
        public EmployeeFilter()
        {
            InitializeComponent();
            mEmployee = new Domain.Employee();
            mDesignations = new List<Domain.Designation>();
            designationService = new DesignationService();
            GetAllDesignations();
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

        private void bdrSearch_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                mEmployee.DeviceUniqueId = txtSearchById.Text;
                mEmployee.Name = txtSearchByName.Text;
                mEmployee.Designation = _designationName;
                this.Close();
                refreshEvent?.Invoke(mEmployee, EventArgs.Empty);
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void bdrReset_MouseDown(object sender, MouseButtonEventArgs e)
        {
            txtSearchByName.Text = string.Empty;
            txtSearchById.Text = string.Empty;
            _designationName = string.Empty;
        }

        private void txtSearchByName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                mEmployee.Name = txtSearchByName.Text;
                this.Close();
                refreshEvent?.Invoke(mEmployee, EventArgs.Empty);
            }
        }
        
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            refreshEvent?.Invoke(mEmployee, EventArgs.Empty);
        }

        private void txtSearchByUniqId_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                mEmployee.DeviceUniqueId = txtSearchById.Text;
                this.Close();
                refreshEvent?.Invoke(mEmployee, EventArgs.Empty);
            }
        }
    }
}
