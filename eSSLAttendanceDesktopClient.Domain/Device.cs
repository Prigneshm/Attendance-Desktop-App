using System;

namespace eSSLAttendanceDesktopClient.Domain
{
    public class Device
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string IPAddress { get; set; }
        public int Port { get; set; }
        //public  bool IsActive { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int BgColor { get; set; }
        public bool DeviceConnected { get; set; }
        private bool _isActive;
        public bool IsActive
        {
            get => _isActive;
            set
            {
                _isActive = value;
                Status = _isActive ? "Active" : "Disabled";
            }
        }

        public string Status { get; private set; } // Backed by IsActive Inactive
    }
}
