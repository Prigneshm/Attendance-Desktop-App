using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eSSLAttendanceDesktopClient.Utility
{
    public static class DesktopAppEnum
    {
        public enum PositionEnum
        {
            HR = 1,
            Manager = 2,
            Accountan = 3,
            Supervisor = 4,
            Technician = 5,
            Worker = 6
        }

        public enum WorkingHourEnm
        {
            [Display(Name = "Eight")]
            Eight = 8,
            [Display(Name = "Twele")]
            Twele = 12,
            [Display(Name = "Ten")]
            Ten = 10,
        }
        public enum MenuEnum
        {
            [Display(Name = "Employee")]
            Employee = 1,
            [Display(Name = "Report")]
            Report = 2,
            [Display(Name = "Connect Device")]
            Connect_Device = 3,
            [Display(Name = "Device")]
            Device = 4,
            [Display(Name = "Attendance_Log")]
            Attendance_Log = 5,
        }
    }
}
