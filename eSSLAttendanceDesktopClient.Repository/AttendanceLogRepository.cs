using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Linq;
using System.Text.Json;
using eSSLAttendanceDesktopClient.Infrastructure;
using eSSLAttendanceDesktopClient.Infrastructure.IRepository;

namespace eSSLAttendanceDesktopClient.Repository
{
    public class AttendanceLogRepository : IAttendanceLogRepository
    {
        public Domain.AttendanceLog Create(Domain.AttendanceLog mAttendanceLog)
        {
            try
            {
                using (var context = new DataAccess.eSSLAttendanceEntities())
                {
                    var efAttendanceLog = context.AttendanceLogs.Where(x => x.Id == mAttendanceLog.Id).FirstOrDefault();
                    if (efAttendanceLog == null)
                    {
                        efAttendanceLog = new DataAccess.AttendanceLog();
                        context.AttendanceLogs.Add(efAttendanceLog);
                        efAttendanceLog.FromDate = DateTime.Now;
                    }
                    efAttendanceLog.EmployeeId = mAttendanceLog.EmployeeId;
                    efAttendanceLog.CheckIn = mAttendanceLog.CheckIn;
                    efAttendanceLog.CheckOut = mAttendanceLog.CheckOut;
                    efAttendanceLog.Overtime = mAttendanceLog.Overtime;

                    context.SaveChanges();
                    mAttendanceLog.Id = efAttendanceLog.Id;
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
            return mAttendanceLog;
        }

        public void BulkInsert(List<Domain.AttendancePair> mPair)
        {
            try
            {
                using (var context = new DataAccess.eSSLAttendanceEntities())
                {
                    foreach (var pair in mPair)
                    {
                        var efAttendanceLog = context.AttendanceLogs.Where(x => x.EmployeeId == pair.EmployeeId && x.CheckIn == pair.CheckIn && x.ToDate == null).FirstOrDefault();
                        if (efAttendanceLog == null)
                        {
                            efAttendanceLog = new DataAccess.AttendanceLog();
                            context.AttendanceLogs.Add(efAttendanceLog);
                            efAttendanceLog.EmployeeId = pair.EmployeeId;
                            efAttendanceLog.CheckIn = pair.CheckIn;
                            efAttendanceLog.FromDate = DateTime.Now;
                        }

                        efAttendanceLog.CheckOut = pair.CheckOut;
                        efAttendanceLog.Overtime = $"{pair.Overtime.Hours:D2}:{pair.Overtime.Minutes:D2}:{pair.Overtime.Seconds:D2}";
                        efAttendanceLog.Status = pair.Status;

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

        public Domain.AttendanceLogLister GetAll(Domain.AttendanceLogLister mLister)
        {
            mLister.List = new List<Domain.AttendanceLog>();
            try
            {
                using (var context = new DataAccess.eSSLAttendanceEntities())
                {
                    var efRecords = (from attendanceLog in context.AttendanceLogs
                                     join employee in context.Employees on attendanceLog.EmployeeId equals employee.Id into joinEmp
                                     from employee in joinEmp.DefaultIfEmpty()
                                     where attendanceLog.ToDate == null
                                     orderby attendanceLog.CheckIn descending
                                     select new Domain.AttendanceLog
                                     {
                                         Id = attendanceLog.Id,
                                         EmployeeId = attendanceLog.EmployeeId,
                                         CheckIn = (DateTime)attendanceLog.CheckIn,
                                         CheckOut = (DateTime)attendanceLog.CheckOut,
                                         Overtime = attendanceLog.Overtime,
                                         Status = attendanceLog.Status,
                                         EmployeeName = employee.FirstName + " " + employee.LastName,
                                         Department = employee.Department.Name,
                                         Designation = employee.Designation.Name,
                                     }).AsEnumerable();

                    if (mLister.SearchCriteria != null)
                    {
                        if (mLister.SearchCriteria.EmployeeName.IsNotNullOrEmpty())
                        {
                            var name = mLister.SearchCriteria.EmployeeName.StrToUpper();
                            efRecords = efRecords.AsQueryable().Where(x => (!(x.EmployeeName == null || x.EmployeeName == string.Empty) && x.EmployeeName.Trim().ToUpper().Contains(name))).AsEnumerable();
                        }
                        
                        if (mLister.SearchCriteria.Status.IsNotNullOrEmpty())
                        {
                            var status = mLister.SearchCriteria.Status.StrToUpper();
                            efRecords = efRecords.AsQueryable().Where(x => (!(x.Status == null || x.Status == string.Empty) && x.Status.Trim().ToUpper().Contains(status))).AsEnumerable();
                        }
                        
                        if (mLister.SearchCriteria.Department.IsNotNullOrEmpty())
                        {
                            var departmentName = mLister.SearchCriteria.Department.StrToUpper();
                            efRecords = efRecords.AsQueryable().Where(x => (!(x.Department == null || x.Department == string.Empty) && x.Department.Trim().ToUpper().Contains(departmentName))).AsEnumerable();
                        }
                        
                        if (mLister.SearchCriteria.Designation.IsNotNullOrEmpty())
                        {
                            var designationName = mLister.SearchCriteria.Designation.StrToUpper();
                            efRecords = efRecords.AsQueryable().Where(x => (!(x.Designation == null || x.Designation == string.Empty) && x.Designation.Trim().ToUpper().Contains(designationName))).AsEnumerable();
                        }

                        if (mLister.SearchCriteria.FromDate != DateTime.MinValue)
                        {
                            var searchFromDate = mLister.SearchCriteria.FromDate.Date;
                            efRecords = efRecords.AsQueryable().Where(x => DbFunctions.TruncateTime(x.CheckIn) >= searchFromDate).AsEnumerable();
                        }

                        if (mLister.SearchCriteria.ToDate.HasValue && mLister.SearchCriteria.ToDate.Value != DateTime.MinValue)
                        {
                            var searchToDate = mLister.SearchCriteria.ToDate.Value.Date;
                            efRecords = efRecords.AsQueryable().Where(x => DbFunctions.TruncateTime(x.CheckOut) <= searchToDate).AsEnumerable();
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
                        mLister.List = JsonSerializer.Deserialize<List<Domain.AttendanceLog>>(JsonSerializer.Serialize(efRecords.ToList()));
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

        public void Delete(int id)
        {
            try
            {
                using (var context = new DataAccess.eSSLAttendanceEntities())
                {
                    var efEmployee = context.AttendanceLogs.Where(x => x.Id == id && x.ToDate == null).FirstOrDefault();

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

        public void Update(int id, Domain.AttendanceLog mAttendanceLog)
        {
            try
            {
                using (var context = new DataAccess.eSSLAttendanceEntities())
                {
                    var efAttendancelog = context.AttendanceLogs.Where(x => x.Id == id).FirstOrDefault();
                    if (efAttendancelog != null && mAttendanceLog != null)
                    {
                        if (mAttendanceLog.EmployeeId > 0 && efAttendancelog.EmployeeId != mAttendanceLog.EmployeeId)
                            efAttendancelog.EmployeeId = mAttendanceLog.EmployeeId;

                        if (mAttendanceLog.CheckIn != null || (!efAttendancelog.CheckIn.HasValue && efAttendancelog.CheckIn.Value != mAttendanceLog.CheckIn.Value))
                            efAttendancelog.CheckIn = mAttendanceLog.CheckIn.Value;

                        if (mAttendanceLog.CheckOut != null || (!efAttendancelog.CheckOut.HasValue && efAttendancelog.CheckOut.Value != mAttendanceLog.CheckOut.Value))
                            efAttendancelog.CheckOut = mAttendanceLog.CheckOut.Value;

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
    }
}
