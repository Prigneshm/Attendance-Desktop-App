using System.Collections.Generic;

namespace eSSLAttendanceDesktopClient.Infrastructure.IRepository
{
    public interface IAttendanceLogRepository
    {
        Domain.AttendanceLog Create(Domain.AttendanceLog mAttendanceLog);

        void BulkInsert(List<Domain.AttendancePair> mPair);
        Domain.AttendanceLogLister GetAll(Domain.AttendanceLogLister mLister);

        void Delete(int id);

        void Update(int id, Domain.AttendanceLog mAttendanceLog);
    }
}
