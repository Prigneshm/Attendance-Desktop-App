using System;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Linq;
using eSSLAttendanceDesktopClient.Infrastructure.IRepository;
using eSSLAttendanceDesktopClient.Infrastructure;
using System.Collections.Generic;
using System.Text.Json;

namespace eSSLAttendanceDesktopClient.Repository
{
    public class DeviceRepository : IDeviceRepository
    {
        public Domain.Device Create(Domain.Device mDevice)
        {
            try
            {
                using (var context = new DataAccess.eSSLAttendanceEntities())
                {
                    var efDevice = context.Devices.Where(x => x.Id == mDevice.Id).FirstOrDefault();
                    if (efDevice == null)
                    {
                        efDevice = new DataAccess.Device();
                        context.Devices.Add(efDevice);
                        efDevice.FromDate = DateTime.Now;
                        efDevice.IsActive = true;
                    }
                    efDevice.Name = mDevice.Name;
                    efDevice.IPAddress = mDevice.IPAddress;
                    efDevice.Port = mDevice.Port;
                    efDevice.IsActive = mDevice.IsActive;
                    context.SaveChanges();
                    mDevice.Id = efDevice.Id;
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
            return mDevice;
        }

        public void Delete(int id)
        {
            try
            {
                using (var context = new DataAccess.eSSLAttendanceEntities())
                {
                    var efDevice = context.Devices.Where(x => x.Id == id && x.ToDate == null).FirstOrDefault();
                    if (efDevice != null && efDevice.Id > default(int))
                    {
                        efDevice.ToDate = DateTime.Now;
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

        public Domain.Device Get(int id)
        {
            var mDevice = new Domain.Device();
            try
            {
                using (var context = new DataAccess.eSSLAttendanceEntities())
                {
                    mDevice = (from device in context.Devices
                               where device.Id == id && device.ToDate == null && device.IsActive
                               select new Domain.Device
                               {
                                   Id = device.Id,
                                   Name = device.Name,
                                   IPAddress = device.IPAddress,
                                   Port = device.Port,
                                   IsActive = device.IsActive,
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
            return mDevice;
        }

        public Domain.DeviceLister GetAll(Domain.DeviceLister mLister)
        {
            mLister.List = new List<Domain.Device>();
            try
            {
                using (var context = new DataAccess.eSSLAttendanceEntities())
                {
                    var efRecords = (from device in context.Devices
                                     where device.ToDate == null
                                     orderby device.Id descending
                                     select new Domain.Device
                                     {
                                         Id = device.Id,
                                         Name = device.Name,
                                         IPAddress = device.IPAddress,
                                         Port = device.Port,
                                         IsActive = device.IsActive,
                                     }).AsEnumerable();

                    if (mLister.SearchCriteria != null)
                    {
                        if (mLister.SearchCriteria.Name.IsNotNullOrEmpty())
                        {
                            var name = mLister.SearchCriteria.Name.StrToUpper();
                            efRecords = efRecords.AsQueryable().Where(x => (!(x.Name == null || x.Name == string.Empty) && x.Name.Trim().ToUpper().Contains(name))).AsEnumerable();
                        }
                        if (mLister.SearchCriteria.IPAddress.IsNotNullOrEmpty())
                        {
                            var ip = mLister.SearchCriteria.IPAddress.StrToUpper();
                            efRecords = efRecords.AsQueryable().Where(x => (!(x.IPAddress == null || x.IPAddress == string.Empty) && x.IPAddress.Trim().ToUpper().Contains(ip))).AsEnumerable();
                        }

                        if (mLister.SearchCriteria.Port > 0)
                        {
                            efRecords = efRecords.AsQueryable().Where(x => x.Port > 0 && x.Port == mLister.SearchCriteria.Port).AsEnumerable();
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
                        mLister.List = JsonSerializer.Deserialize<List<Domain.Device>>(JsonSerializer.Serialize(efRecords.ToList()));
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

        public List<Domain.Device> GetAllActiveDevices()
        {
            var mDevices = new List<Domain.Device>();
            try
            {
                using (var context = new DataAccess.eSSLAttendanceEntities())
                {
                    var efDevices = (from device in context.Devices
                                     where device.ToDate == null && device.IsActive
                                     select new
                                     {
                                         Id = device.Id,
                                         Name = device.Name
                                     }).AsEnumerable();

                    mDevices = JsonSerializer.Deserialize<List<Domain.Device>>(JsonSerializer.Serialize(efDevices.ToList()));
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
            return mDevices;
        }

        public void Update(int id, Domain.Device mDevice)
        {
            try
            {
                using (var context = new DataAccess.eSSLAttendanceEntities())
                {
                    var efDevice = context.Devices.Where(x => x.Id == id).FirstOrDefault();
                    if (efDevice != null && mDevice != null)
                    {
                        if (mDevice.Name.IsNotNullOrEmpty() && efDevice.Name != mDevice.Name)
                            efDevice.Name = mDevice.Name;

                        if (mDevice.IPAddress.IsNotNullOrEmpty() && efDevice.IPAddress != mDevice.IPAddress)
                            efDevice.IPAddress = mDevice.IPAddress;

                        if (mDevice.Port > 0 && efDevice.Port != mDevice.Port)
                            efDevice.Port = mDevice.Port;

                        if (efDevice.IsActive != mDevice.IsActive)
                            efDevice.IsActive = mDevice.IsActive;

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
