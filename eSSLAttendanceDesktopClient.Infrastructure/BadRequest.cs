using System;

namespace eSSLAttendanceDesktopClient.Infrastructure
{
    [Serializable]
    public class BadRequest : Exception
    {
        public BadRequest() { }

        public BadRequest(string errorMsg)
            : base(errorMsg)
        {

        }
    }
}
