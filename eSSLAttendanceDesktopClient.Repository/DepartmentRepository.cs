using eSSLAttendanceDesktopClient.Infrastructure;
using eSSLAttendanceDesktopClient.Infrastructure.IRepository;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Linq;
using System.Text.Json;

namespace eSSLAttendanceDesktopClient.Repository
{
    public class DepartmentRepository : IDepartmentRepository
    {
        public List<Domain.Department> GetAllActiveDepartments()
        {
            var mDepartments = new List<Domain.Department>();
            try
            {
                using (var context = new DataAccess.eSSLAttendanceEntities())
                {
                    var efDepartments = (from department in context.Departments
                                         where department.ToDate == null && department.IsActive
                                         select new Domain.Department
                                         {
                                             Id = department.Id,
                                             Name = department.Name
                                         }).AsEnumerable();

                    mDepartments = JsonSerializer.Deserialize<List<Domain.Department>>(JsonSerializer.Serialize(efDepartments.ToList()));
                }
            }
            catch (DbUpdateException ex)
            {
                var entityException = ex.InnerException.InnerException as SqlException;
                if (entityException != null)
                    throw new Exception(entityException.Message);
            }
            catch (DbEntityValidationException ex)
            {
                throw new Exception(ex.GetValidationErrors());
            }
            catch (Exception)
            {
                throw;
            }
            return mDepartments;
        }

        public Domain.Department GetById(int id)
        {
            var mDepartment = new Domain.Department();
            try
            {
                using (var context = new DataAccess.eSSLAttendanceEntities())
                {
                    mDepartment = (from department in context.Departments
                               where department.Id == id && department.ToDate == null && department.IsActive
                               select new Domain.Department
                               {
                                   Id = department.Id,
                                   Name = department.Name,
                               }).FirstOrDefault();
                }
            }
            catch (DbUpdateException ex)
            {
                var entityException = ex.InnerException.InnerException as SqlException;
                if (entityException != null)
                    throw new Exception(entityException.Message);
            }
            catch (DbEntityValidationException ex)
            {
                throw new Exception(ex.GetValidationErrors());
            }
            catch (Exception)
            {
                throw;
            }
            return mDepartment;
        }
    }
}
