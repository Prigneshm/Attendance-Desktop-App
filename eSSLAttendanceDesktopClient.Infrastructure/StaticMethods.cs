using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eSSLAttendanceDesktopClient.Infrastructure
{
    public static class StaticMethods
    {
        public static DateTime ToDateTime(string inputtedValue)
        {
            var convertedDateTime = DateTime.MinValue;

            string format = "dd/MM/yyyy HH:mm:ss";

            if (DateTime.TryParseExact(inputtedValue, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate))
            {
               convertedDateTime = parsedDate;
            }

            return convertedDateTime;
        }
    }
}
