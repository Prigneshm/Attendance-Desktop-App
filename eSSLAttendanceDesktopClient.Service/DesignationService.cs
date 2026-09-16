using eSSLAttendanceDesktopClient.Domain;
using eSSLAttendanceDesktopClient.Infrastructure.IRepository;
using eSSLAttendanceDesktopClient.Infrastructure.IService;
using eSSLAttendanceDesktopClient.Repository;
using System;
using System.Collections.Generic;
using System.Linq;

namespace eSSLAttendanceDesktopClient.Service
{
    public class DesignationService : IDesignationService
    {
        private readonly IDesignationRepository _repository;

        public DesignationService()
        {
            _repository = new DesignationRepository();
        }

        public List<Domain.Designation> GetAllActiveDesignations(int departmentId)
        {
            return _repository.GetAllActiveDesignations(departmentId);
        }

        public List<Designation> GetAllDesignations()
        {
            return _repository.GetAllDesignations();
        }

        public Domain.Designation GetById(int id)
        {
            return _repository.GetById(id);
        }
    }
}
