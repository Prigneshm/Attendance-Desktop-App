using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eSSLAttendanceDesktopClient.Model
{
    public class AttendanceFilter
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Designation { get; set; }
        public int StartDate { get; set; }
        public int EndDate { get; set; }
    }
}
