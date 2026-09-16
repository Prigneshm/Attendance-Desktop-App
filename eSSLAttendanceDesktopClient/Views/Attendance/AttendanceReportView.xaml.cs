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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace eSSLAttendanceDesktopClient.Views.Attendance
{
    /// <summary>
    /// Interaction logic for AttendanceReportView.xaml
    /// </summary>
    public partial class AttendanceReportView : UserControl
    {
        #region [Objects]
        private List<Model.AttendanceReport> mAttendanceReports;
        #endregion

        #region [CTOR]
        public AttendanceReportView()
        {
            InitializeComponent();
            mAttendanceReports = new List<Model.AttendanceReport>();
            GetReportDatas();
        }
        #endregion

        #region [Methods]
        private void GetReportDatas()
        {
            mAttendanceReports.Clear();
            mAttendanceReports.Add(new Model.AttendanceReport()
            {
                Id = 1,
                SirialNumber = "01",
                Name = "Alex smith",
                Status = "On Time",
                CheckIn = DateTime.Now.Date,
                CheckOut = DateTime.Now.Date,
                RequiredTime = "9:00 Hours",
                ActuleTime = "9 Hours 00 Min",
                BgColor = 1
            });
            mAttendanceReports.Add(new Model.AttendanceReport()
            {
                Id = 2,
                SirialNumber = "02",
                Name = "Alex smith",
                Status = "Absent",
                CheckIn = DateTime.Now,
                CheckOut = DateTime.Now,
                RequiredTime = "9:00 Hours",
                ActuleTime = "-",
                BgColor = 2
            });
            mAttendanceReports.Add(new Model.AttendanceReport()
            {
                Id = 3,
                SirialNumber = "03",
                Name = "Alex smith",
                Status = "Late",
                CheckIn = DateTime.Now,
                CheckOut = DateTime.Now,
                RequiredTime = "9:00 Hours",
                ActuleTime = "9 Hours 56 Min",
                BgColor = 1
            });
            mAttendanceReports.Add(new Model.AttendanceReport()
            {
                Id = 4,
                SirialNumber = "04",
                Name = "Alex smith",
                Status = "On Time",
                CheckIn = DateTime.Now,
                CheckOut = DateTime.Now,
                RequiredTime = "9:00 Hours",
                ActuleTime = "9 Hours 00 Min",
                BgColor = 2
            });
            mAttendanceReports.Add(new Model.AttendanceReport()
            {
                Id = 5,
                SirialNumber = "05",
                Name = "Alex smith",
                Status = "Absent",
                CheckIn = DateTime.Now.Date,
                CheckOut = DateTime.Now.Date,
                RequiredTime = "9:00 Hours",
                ActuleTime = "-",
                BgColor = 1
            });
            mAttendanceReports.Add(new Model.AttendanceReport()
            {
                Id = 5,
                SirialNumber = "06",
                Name = "Alex smith",
                Status = "On Time",
                CheckIn = DateTime.Now,
                CheckOut = DateTime.Now,
                RequiredTime = "9:00 Hours",
                ActuleTime = "9 Hours 00 Min",
                BgColor = 2
            });
            mAttendanceReports.Add(new Model.AttendanceReport()
            {
                Id = 5,
                SirialNumber = "07",
                Name = "Alex smith",
                Status = "Late",
                CheckIn = DateTime.Now,
                CheckOut = DateTime.Now,
                RequiredTime = "9:00 Hours",
                ActuleTime = "-",
                BgColor = 1
            });
            mAttendanceReports.Add(new Model.AttendanceReport()
            {
                Id = 5,
                SirialNumber = "08",
                Name = "Alex smith",
                Status = "On Time",
                CheckIn = DateTime.Now,
                CheckOut = DateTime.Now,
                RequiredTime = "9:00 Hours",
                ActuleTime = "9 Hours 00 Min",
                BgColor = 2
            });
            mAttendanceReports.Add(new Model.AttendanceReport()
            {
                Id = 5,
                SirialNumber = "09",
                Name = "Alex smith",
                Status = "Absent",
                CheckIn = DateTime.Now,
                CheckOut = DateTime.Now,
                RequiredTime = "9:00 Hours",
                ActuleTime = "-",
                BgColor = 1
            });
            mAttendanceReports.Add(new Model.AttendanceReport()
            {
                Id = 5,
                SirialNumber = "10",
                Name = "Alex smith",
                Status = "Late",
                CheckIn = DateTime.Now,
                CheckOut = DateTime.Now,
                RequiredTime = "9:00 Hours",
                ActuleTime = "9 Hours 00 Min",
                BgColor = 2
            });
            lstAttendanceReport.ItemsSource = mAttendanceReports.ToList();
        } 
        #endregion
    }
}
