using eSSLAttendanceDesktopClient.Domain;
using System.Collections.Generic;

namespace eSSLAttendanceDesktopClient.Infrastructure.IService
{
    public interface IAttendanceService
    {
        Domain.Device ConnectDevice(Domain.Device mDevice);

        List<AttendanceLog> GetAttendanceLogs(Domain.Device mDevice);

        Domain.Device DisconnectDevice(Domain.Device mDevice);
    }
}