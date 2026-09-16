using System.Collections.Generic;
using System.Linq;
using eSSLAttendanceDesktopClient.Domain;
using eSSLAttendanceDesktopClient.Infrastructure;
using eSSLAttendanceDesktopClient.Infrastructure.IRepository;
using eSSLAttendanceDesktopClient.Infrastructure.IService;
using eSSLAttendanceDesktopClient.Repository;

namespace eSSLAttendanceDesktopClient.Service
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _repository;

        public EmployeeService()
        {
            _repository = new EmployeeRepository();
        }

        public Domain.Employee Create(Domain.Employee mEmployee)
        {
            return _repository.Create(mEmployee);
        }

        public Domain.Employee Get(int id)
        {
            return _repository.Get(id);
        }

        public void Delete(int id)
        {
            _repository.Delete(id);
        }

        public void Update(int id, Domain.Employee mEmployee)
        {
            if (mEmployee != null)
            {
                _repository.Update(id, mEmployee);
            }
            else
                throw new BadRequest("The Object must have a a value");
        }

        public Domain.EmployeeLister GetAll(Domain.EmployeeLister mLister)
        {
            try
            {
                mLister = _repository.GetAll(mLister);
                if (mLister != null && mLister.List != null && mLister.List.Count > default(int))
                {
                    mLister.List = mLister.List.Select((employee, index) =>
                    {
                        employee.BgColor = index % 2 == 0 ? 2 : 1;
                        return employee;
                    }).ToList();
                }
                return mLister;
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        public List<Employee> GetAll()
        {
            return _repository.GetAll();
        }
    }
}
