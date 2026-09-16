using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eSSLAttendanceDesktopClient.Model
{
    public class AttendanceReport
    {
        public int Id { get; set; }
        public string SirialNumber { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public string RequiredTime { get; set; }
        public string ActuleTime { get; set; }
        public int BgColor { get; set; }
    }
}
