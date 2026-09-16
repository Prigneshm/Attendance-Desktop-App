using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eSSLAttendanceDesktopClient.Domain
{
    public class AttendancePair
    {
        public int EmployeeId { get; set; }
        public DateTime? CheckIn { get; set; }
        public DateTime? CheckOut { get; set; }
        public string Status { get; set; }
        public int WorkingHours { get; set; }
        public TimeSpan Overtime { get; set; }
    }
}
