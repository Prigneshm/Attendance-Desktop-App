using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eSSLAttendanceDesktopClient.Utility
{
    public static class CsvHelper
    {
        public static string ToCsv<T>(IEnumerable<T> items)
        {
            var csv = new StringBuilder();
            var props = typeof(T).GetProperties();

            // Header
            csv.AppendLine(string.Join(",", props.Select(p => p.Name)));

            // Rows
            foreach (var item in items)
            {
                var values = props.Select(p => $"\"{p.GetValue(item, null)?.ToString()?.Replace("\"", "\"\"")}\"");
                csv.AppendLine(string.Join(",", values));
            }

            return csv.ToString();
        }
    }

}
