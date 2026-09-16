using System.Collections.Generic;

namespace eSSLAttendanceDesktopClient.Infrastructure.IRepository
{
    public interface IDepartmentRepository
    {
        List<Domain.Department> GetAllActiveDepartments();
        Domain.Department GetById(int id);
    }
}
