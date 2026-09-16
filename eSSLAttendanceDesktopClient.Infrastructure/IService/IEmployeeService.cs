using System.Collections.Generic;

namespace eSSLAttendanceDesktopClient.Infrastructure.IService
{
    public interface IEmployeeService
    {
        Domain.Employee Create(Domain.Employee mEmployee);

        Domain.Employee Get(int id);

        void Delete(int id);

        void Update(int id, Domain.Employee mEmployee);

        Domain.EmployeeLister GetAll(Domain.EmployeeLister mLister);

        List<Domain.Employee> GetAll();
    }
}
