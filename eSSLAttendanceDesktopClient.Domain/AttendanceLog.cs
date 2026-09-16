using System;
using System.Security.Permissions;

namespace eSSLAttendanceDesktopClient.Domain
{
    public class AttendanceLog
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public DateTime? CheckIn { get; set; }
        public DateTime? CheckOut { get; set; }
        public string Status { get; set; }
        public int WorkingHours { get; set; }
        public string Overtime { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int Count { get; set; }
        public string EnrollNumber { get; set; }
        public int VerifyMode { get; set; }
        public int InOutMode { get; set; }
        public DateTime Timestamp { get; set; }
        public int WorkCode { get; set; }
        public int Index { get; set; }

        //Extra
        public string EmployeeName { get; set; }
        public string Department { get; set; }
        public string Designation { get; set; }

        public int BgColor { get; set; }
        public string OvertimeFormatted => Overtime == "00:00:00" ? "-" : Overtime;
        public int PageNumber { get; set; }
        public string WorkingHoursFormatted
        {
            get
            {
                if (CheckIn.HasValue && CheckOut.HasValue)
                {
                    TimeSpan duration = CheckOut.Value - CheckIn.Value;
                    return $"{(int)duration.TotalHours:D2}:{duration.Minutes:D2}";
                }
                return "-";
            }
        }
    }
}
