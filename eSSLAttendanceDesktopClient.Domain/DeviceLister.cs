using System.Collections.Generic;

namespace eSSLAttendanceDesktopClient.Domain
{
    public class DeviceLister
    {
        public List<Domain.Device> List { get; set; } = new List<Domain.Device>();
        public Domain.Device SearchCriteria { get; set; } = new Domain.Device();
        public Pagination Pagination { get; set; } = new Pagination();
    }
}
