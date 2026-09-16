using eSSLAttendanceDesktopClient.Infrastructure.IRepository;
using eSSLAttendanceDesktopClient.Infrastructure.IService;
using eSSLAttendanceDesktopClient.Repository;
using System.Collections.Generic;

namespace eSSLAttendanceDesktopClient.Service
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IDepartmentRepository _repository;

        public DepartmentService()
        {
            _repository = new DepartmentRepository();
        }

        public List<Domain.Department> GetAllActiveDepartments()
        {
            return _repository.GetAllActiveDepartments();
        }

        public Domain.Department GetById(int id)
        {
            return _repository.GetById(id);
        }
    }
}
