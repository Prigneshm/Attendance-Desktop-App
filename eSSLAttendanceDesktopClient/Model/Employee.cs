using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace eSSLAttendanceDesktopClient.Model
{
    public class Employee
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Contact { get; set; }
        public int WorkingHours { get; set; }
        public string DeviceUniqueId { get; set; }
        public string Position { get; set; }
        public bool IsActive { get; set; }
        public int PositionId { get; set; }

        //Extra
        public string SerialNumber { get; set; }
        public string Name { get; set; }
        [JsonIgnore]
        public int BgColor { get; set; }
        
        [JsonIgnore]
        public string FullName
        {
            get { return $"{FirstName} {LastName}"; }
        }
    }
}
