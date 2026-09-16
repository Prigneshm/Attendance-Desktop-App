using System.Collections.Generic;

namespace eSSLAttendanceDesktopClient.Infrastructure.IRepository
{
    public interface IDeviceRepository
    {
        Domain.Device Create(Domain.Device mDevice);

        Domain.Device Get(int id);

        void Delete(int id);

        void Update(int id, Domain.Device mDevice);

        Domain.DeviceLister GetAll(Domain.DeviceLister mLister);

        List<Domain.Device> GetAllActiveDevices();
    }
}
