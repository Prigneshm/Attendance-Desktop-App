using System.Collections.Generic;

namespace eSSLAttendanceDesktopClient.Domain
{
    public class EmployeeLister
    {
        public List<Employee> List { get; set; } = new List<Employee>();
        public Employee SearchCriteria { get; set; } = new Employee();
        public Pagination Pagination { get; set; } = new Pagination();
    }
}
