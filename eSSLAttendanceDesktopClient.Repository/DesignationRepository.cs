using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Linq;
using System.Text.Json;
using eSSLAttendanceDesktopClient.DataAccess;
using eSSLAttendanceDesktopClient.Domain;
using eSSLAttendanceDesktopClient.Infrastructure;
using eSSLAttendanceDesktopClient.Infrastructure.IRepository;

namespace eSSLAttendanceDesktopClient.Repository
{
    public class DesignationRepository : IDesignationRepository
    {
        public List<Domain.Designation> GetAllActiveDesignations(int departmentId)
        {
            var mDesignations = new List<Domain.Designation>();
            try
            {
                using (var context = new DataAccess.eSSLAttendanceEntities())
                {
                    var efDesignations = (from designation in context.Designations
                                         where designation.ToDate == null && designation.IsActive && designation.DepartmentId == departmentId
                                          select new Domain.Designation
                                         {
                                             Id = designation.Id,
                                             DepartmentId = designation.DepartmentId,
                                             Name = designation.Name
                                         }).AsEnumerable();

                    mDesignations = JsonSerializer.Deserialize<List<Domain.Designation>>(JsonSerializer.Serialize(efDesignations.ToList()));
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
            return mDesignations;
        }

        public Domain.Designation GetById(int id)
        {
            var mDesignation = new Domain.Designation();
            try
            {
                using (var context = new DataAccess.eSSLAttendanceEntities())
                {
                    mDesignation = (from designation in context.Designations
                                   where designation.Id == id && designation.ToDate == null && designation.IsActive
                                   select new Domain.Designation
                                   {
                                       Id = designation.Id,
                                       Name = designation.Name,
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
            return mDesignation;
        }

        List<Domain.Designation> IDesignationRepository.GetAllDesignations()
        {
            var mDesignations = new List<Domain.Designation>();
            try
            {
                using (var context = new DataAccess.eSSLAttendanceEntities())
                {
                    var efDesignations = (from designation in context.Designations
                                          where designation.ToDate == null && designation.IsActive 
                                          select new Domain.Designation
                                          {
                                              Id = designation.Id,
                                              DepartmentId = designation.DepartmentId,
                                              Name = designation.Name
                                          }).AsEnumerable();

                    mDesignations = JsonSerializer.Deserialize<List<Domain.Designation>>(JsonSerializer.Serialize(efDesignations.ToList()));
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
            return mDesignations;
        }
    }
}
