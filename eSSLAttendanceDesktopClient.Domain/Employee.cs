using System;

namespace eSSLAttendanceDesktopClient.Domain
{
    public class Employee
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Contact { get; set; }
        public int WorkingHours { get; set; }
        public string DeviceUniqueId { get; set; }
        public int? DeviceId { get; set; }
        public int? DepartmentId { get; set; }
        public int? DesignationId { get; set; }
        public string Designation { get; set; }
        public string Department { get; set; }
        public string Device { get; set; }
        public bool IsActive { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime? ToDate { get; set; }

        //Extra
        public int BgColor { get; set; }

        public string EmployeeName
        {
            get { return $"{FirstName} {LastName}"; }
        }
    }
}
