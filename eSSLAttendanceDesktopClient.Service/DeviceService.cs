using System.Collections.Generic;
using System.Linq;
using eSSLAttendanceDesktopClient.Infrastructure;
using eSSLAttendanceDesktopClient.Infrastructure.IRepository;
using eSSLAttendanceDesktopClient.Infrastructure.IService;
using eSSLAttendanceDesktopClient.Repository;

namespace eSSLAttendanceDesktopClient.Service
{
    public class DeviceService : IDeviceService
    {
        private readonly IDeviceRepository _repository;

        public DeviceService()
        {
            _repository = new DeviceRepository();
        }

        public Domain.Device Create(Domain.Device mDevice)
        {
            return _repository.Create(mDevice);
        }

        public void Delete(int id)
        {
            _repository.Delete(id);
        }

        public Domain.Device Get(int id)
        {
            return _repository.Get(id);
        }

        public Domain.DeviceLister GetAll(Domain.DeviceLister mLister)
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

        public List<Domain.Device> GetDeviceList()
        {
            return _repository.GetAllActiveDevices();
        }

        public void Update(int id, Domain.Device mDevice)
        {
            if (mDevice != null)
            {
                _repository.Update(id, mDevice);
            }
            else
                throw new BadRequest("The Object must have a a value");
        }
    }
}
