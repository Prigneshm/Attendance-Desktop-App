using eSSLAttendanceDesktopClient.Domain;
using System.Collections.Generic;

namespace eSSLAttendanceDesktopClient.Infrastructure.IService
{
    public interface IAttendanceLogService 
    {
        Domain.AttendanceLog Create(AttendanceLog mAttendanceLog);

        void ProcessLog(Domain.Device mDevice, List<Domain.AttendanceLog> mAttendanceLog);

        Domain.AttendanceLogLister GetAll(Domain.AttendanceLogLister mLister);

        void Delete(int id);

        void Update(int id, Domain.AttendanceLog mAttendanceLog);
    }
}
