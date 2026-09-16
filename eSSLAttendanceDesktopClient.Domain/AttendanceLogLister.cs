using System.Collections.Generic;

namespace eSSLAttendanceDesktopClient.Domain
{
    public class AttendanceLogLister
    {
        public List<AttendanceLog> List { get; set; } = new List<AttendanceLog>();
        public AttendanceLog SearchCriteria { get; set; } = new AttendanceLog();
        public Pagination Pagination { get; set; } = new Pagination();
    }
}
