using System.Collections.Generic;

namespace eSSLAttendanceDesktopClient.Infrastructure.IService
{
    public interface IDepartmentService
    {
        List<Domain.Department> GetAllActiveDepartments();
        Domain.Department GetById(int id);
    }
}
