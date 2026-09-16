using System.Collections.Generic;

namespace eSSLAttendanceDesktopClient.Infrastructure.IRepository
{
    public interface IEmployeeRepository
    {
        Domain.Employee Create(Domain.Employee mEmployee);

        Domain.Employee Get(int id);

        void Delete(int id);

        void Update(int id, Domain.Employee mEmployee);

        Domain.EmployeeLister GetAll(Domain.EmployeeLister mLister);

        List<Domain.Employee> GetAll();

        List<Domain.Employee> GetAllBy(int deviceId);
    }
}
