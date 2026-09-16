using System.Collections.Generic;

namespace eSSLAttendanceDesktopClient.Infrastructure.IService
{
    public interface IDesignationService
    {
        List<Domain.Designation> GetAllActiveDesignations(int departmentId);
        List<Domain.Designation> GetAllDesignations();
        Domain.Designation GetById(int id);
    }
}
