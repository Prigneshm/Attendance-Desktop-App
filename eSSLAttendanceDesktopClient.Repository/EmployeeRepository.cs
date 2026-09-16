using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using eSSLAttendanceDesktopClient.Domain;
using eSSLAttendanceDesktopClient.Infrastructure;
using eSSLAttendanceDesktopClient.Infrastructure.IRepository;

namespace eSSLAttendanceDesktopClient.Repository
{
    public class EmployeeRepository : IEmployeeRepository
    {

        public Domain.Employee Create(Domain.Employee mEmployee)
        {
            try
            {
                using (var context = new DataAccess.eSSLAttendanceEntities())
                {
                    var efEmployee = context.Employees.Where(x => x.Id == mEmployee.Id).FirstOrDefault();
                    if (efEmployee == null)
                    {
                        efEmployee = new DataAccess.Employee();
                        context.Employees.Add(efEmployee);
                        efEmployee.FromDate = DateTime.Now;
                        efEmployee.IsActive = true;
                    }
                    efEmployee.FirstName = mEmployee.FirstName;
                    efEmployee.LastName = mEmployee.LastName;
                    efEmployee.Contact = mEmployee.Contact;
                    efEmployee.WorkingHours = mEmployee.WorkingHours;
                    efEmployee.DeviceUniqueId = mEmployee.DeviceUniqueId;
                    efEmployee.DeviceId = mEmployee.DeviceId;
                    efEmployee.DepartmentId = mEmployee.DepartmentId;
                    efEmployee.DesignationId = mEmployee.DesignationId;

                    context.SaveChanges();
                    mEmployee.Id = efEmployee.Id;
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
            return mEmployee;
        }

        public Domain.Employee Get(int id)
        {
            var mEmployee = new Domain.Employee();
            try
            {
                using (var context = new DataAccess.eSSLAttendanceEntities())
                {
                    mEmployee = (from employee in context.Employees
                                 where employee.Id == id && employee.ToDate == null && employee.IsActive
                                 select new Domain.Employee
                                 {
                                     Id = employee.Id,
                                     FirstName = employee.FirstName,
                                     LastName = employee.LastName,
                                     Contact = employee.Contact,
                                     WorkingHours = employee.WorkingHours,
                                     DeviceUniqueId = employee.DeviceUniqueId,
                                     DeviceId = employee.DeviceId,

                                     IsActive = employee.IsActive,
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
            return mEmployee;
        }

        public void Delete(int id)
        {
            try
            {
                using (var context = new DataAccess.eSSLAttendanceEntities())
                {
                    var efEmployee = context.Employees.Where(x => x.Id == id && x.ToDate == null).FirstOrDefault();

                    if (efEmployee != null)
                    {
                        efEmployee.ToDate = DateTime.Now;
                        context.SaveChanges();
                    }
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
        }

        public void Update(int id, Domain.Employee mEmployee)
        {
            try
            {
                using (var context = new DataAccess.eSSLAttendanceEntities())
                {
                    var efEmployee = context.Employees.Where(x => x.Id == id).FirstOrDefault();
                    if (efEmployee != null && mEmployee != null)
                    {
                        if (mEmployee.FirstName.IsNotNullOrEmpty() && efEmployee.FirstName != mEmployee.FirstName)
                            efEmployee.FirstName = mEmployee.FirstName;

                        if (mEmployee.LastName.IsNotNullOrEmpty() && efEmployee.LastName != mEmployee.LastName)
                            efEmployee.LastName = mEmployee.LastName;

                        if (mEmployee.Contact.IsNotNullOrEmpty() && efEmployee.Contact != mEmployee.Contact)
                            efEmployee.Contact = mEmployee.Contact;

                        if (mEmployee.WorkingHours > default(int) && efEmployee.WorkingHours != mEmployee.WorkingHours)
                            efEmployee.WorkingHours = mEmployee.WorkingHours;

                        if (mEmployee.DeviceUniqueId.IsNotNullOrEmpty() && efEmployee.DeviceUniqueId != mEmployee.DeviceUniqueId)
                            efEmployee.DeviceUniqueId = mEmployee.DeviceUniqueId;

                        if (mEmployee.DeviceId > default(int) && efEmployee.DeviceId != mEmployee.DeviceId)
                            efEmployee.DeviceId = mEmployee.DeviceId;

                        if (mEmployee.DepartmentId > default(int) && efEmployee.DepartmentId != mEmployee.DepartmentId)
                            efEmployee.DepartmentId = mEmployee.DepartmentId;

                        if (mEmployee.DesignationId > default(int) && efEmployee.DesignationId != mEmployee.DesignationId)
                            efEmployee.DesignationId = mEmployee.DesignationId;

                        if (efEmployee.IsActive != mEmployee.IsActive)
                            efEmployee.IsActive = mEmployee.IsActive;

                        context.SaveChanges();
                    }
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
        }

        public Domain.EmployeeLister GetAll(Domain.EmployeeLister mLister)
        {
            mLister.List = new List<Domain.Employee>();
            try
            {
                using (var context = new DataAccess.eSSLAttendanceEntities())
                {
                    var efRecords = (from employee in context.Employees
                                     join device in context.Devices on employee.DeviceId equals device.Id into joinED
                                     from device in joinED.DefaultIfEmpty()
                                     join department in context.Departments on employee.DepartmentId equals department.Id into joinEDEPT
                                     from department in joinEDEPT.DefaultIfEmpty()
                                     join designation in context.Designations on employee.DesignationId equals designation.Id into joinEDES
                                     from designation in joinEDES.DefaultIfEmpty()
                                     where employee.ToDate == null
                                     orderby employee.Id descending
                                     select new
                                     {
                                         Id = employee.Id,
                                         Name = employee.FirstName + " " + employee.LastName,
                                         FirstName = employee.FirstName,
                                         LastName = employee.LastName,
                                         Contact = employee.Contact,
                                         WorkingHours = employee.WorkingHours,
                                         DeviceUniqueId = employee.DeviceUniqueId,
                                         Device = device.Name,
                                         DeviceId = employee.DeviceId,
                                         Department = department.Name,
                                         DepartmentId = employee.DepartmentId,
                                         Designation = designation.Name,
                                         DesignationId = employee.DeviceId,
                                         IsActive = employee.IsActive,

                                     }).AsEnumerable();

                    if (mLister.SearchCriteria != null)
                    {
                        if (mLister.SearchCriteria.Name.IsNotNullOrEmpty())
                        {
                            var name = mLister.SearchCriteria.Name.StrToUpper();
                            efRecords = efRecords.AsQueryable().Where(x => (!(x.Name == null || x.Name == string.Empty) && x.Name.Trim().ToUpper().Contains(name))).AsEnumerable();
                        }

                        if (mLister.SearchCriteria.DeviceUniqueId.IsNotNullOrEmpty())
                        {
                            var deviceUniqueId = mLister.SearchCriteria.DeviceUniqueId.StrToUpper();
                            efRecords = efRecords.AsQueryable().Where(x => !(x.DeviceUniqueId == null || x.DeviceUniqueId == string.Empty) && x.DeviceUniqueId.Trim().ToUpper().Contains(deviceUniqueId)).AsEnumerable();
                        }
                        
                        if (mLister.SearchCriteria.Device.IsNotNullOrEmpty())
                        {
                            var Device = mLister.SearchCriteria.Device.StrToUpper();
                            efRecords = efRecords.AsQueryable().Where(x => !(x.Device == null || x.Device == string.Empty) && x.Device.Trim().ToUpper().Contains(Device)).AsEnumerable();
                        } 
                        
                        if (mLister.SearchCriteria.Contact.IsNotNullOrEmpty())
                        {
                            var Contact = mLister.SearchCriteria.Contact.StrToUpper();
                            efRecords = efRecords.AsQueryable().Where(x => !(x.Contact == null || x.Contact == string.Empty) && x.Contact.Trim().ToUpper().Contains(Contact)).AsEnumerable();
                        }

                        if (mLister.SearchCriteria.Designation.IsNotNullOrEmpty())
                        {
                            var position = mLister.SearchCriteria.Designation.StrToUpper();
                            efRecords = efRecords.AsQueryable().Where(x => !(x.Designation == null || x.Designation == string.Empty) && x.Designation.Trim().ToUpper().Contains(position)).AsEnumerable();
                        }
                        
                        if (mLister.SearchCriteria.Department.IsNotNullOrEmpty())
                        {
                            var position = mLister.SearchCriteria.Department.StrToUpper();
                            efRecords = efRecords.AsQueryable().Where(x => !(x.Department == null || x.Department == string.Empty) && x.Department.Trim().ToUpper().Contains(position)).AsEnumerable();
                        }
                    }

                    if (mLister.Pagination.TotalRecord == 0)
                    {
                        mLister.Pagination.TotalRecord = efRecords.AsQueryable().Count();
                    }

                    if (mLister.Pagination.Take == -1)
                    {
                        mLister.Pagination.Take = mLister.Pagination.TotalRecord;
                        mLister.Pagination.Skip = 0;
                    }
                    else if (mLister.Pagination.Take > 0)
                    {
                        mLister.Pagination.CurrentPage = (mLister.Pagination.Skip / mLister.Pagination.Take) + 1;
                    }
                    else
                    {
                        mLister.Pagination.CurrentPage = 1;
                    }

                    if (mLister.Pagination.TotalRecord > 0 && mLister.Pagination.Take > 0)
                    {
                        mLister.Pagination.TotalPage = (int)Math.Ceiling(Convert.ToDouble(Decimal.Divide(mLister.Pagination.TotalRecord, mLister.Pagination.Take)));
                    }

                    if (mLister.Pagination.TotalPage == 0)
                    {
                        mLister.Pagination.TotalPage = 1;
                    }

                    if (mLister.Pagination.TotalRecord > 0)
                    {
                        efRecords = efRecords.AsQueryable().Skip(mLister.Pagination.Skip).Take(mLister.Pagination.Take).AsEnumerable();
                        mLister.List = JsonSerializer.Deserialize<List<Domain.Employee>>(JsonSerializer.Serialize(efRecords.ToList()));
                    }
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
            return mLister;
        }

        public List<Employee> GetAll()
        {
            var mEmployees = new List<Domain.Employee>();
            try
            {
                using (var context = new DataAccess.eSSLAttendanceEntities())
                {
                    mEmployees = (from employee in context.Employees
                                  where employee.ToDate == null && employee.IsActive
                                  select new Domain.Employee
                                  {
                                      Id = employee.Id,
                                      FirstName = employee.FirstName,
                                      LastName = employee.LastName,
                                      Contact = employee.Contact,
                                      WorkingHours = employee.WorkingHours,
                                      DeviceUniqueId = employee.DeviceUniqueId,
                                      DeviceId = employee.DeviceId,
                                      IsActive = employee.IsActive,
                                      Name = employee.FirstName + " " + employee.LastName,
                                  }).ToList();
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
            return mEmployees;
        }

        public List<Employee> GetAllBy(int deviceId)
        {
            var mEmployees = new List<Domain.Employee>();
            try
            {
                using (var context = new DataAccess.eSSLAttendanceEntities())
                {
                    mEmployees = (from employee in context.Employees
                                  where employee.ToDate == null && employee.IsActive && employee.DeviceId.HasValue && employee.DeviceId.Value == deviceId
                                  select new Domain.Employee
                                  {
                                      Id = employee.Id,
                                      FirstName = employee.FirstName,
                                      LastName = employee.LastName,
                                      Contact = employee.Contact,
                                      WorkingHours = employee.WorkingHours,
                                      DeviceUniqueId = employee.DeviceUniqueId,
                                      DeviceId = employee.DeviceId,
                                      IsActive = employee.IsActive,
                                  }).ToList();
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
            return mEmployees;
        }
    }
}
