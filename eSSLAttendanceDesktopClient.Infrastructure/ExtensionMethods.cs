using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace eSSLAttendanceDesktopClient.Infrastructure
{
    public static class ExtensionMethods
    {
        public static bool IsNotNullOrEmpty(this string inputtedString)
        {
            return !string.IsNullOrEmpty(inputtedString) && !string.IsNullOrWhiteSpace(inputtedString);
        }

        public static bool IsNullOrEmpty(this string inputtedString)
        {
            return string.IsNullOrEmpty(inputtedString) || string.IsNullOrWhiteSpace(inputtedString);
        }

        public static string ToUpperCase(this string inputtedString)
        {
            if (inputtedString.IsNotNullOrEmpty())
            {
                inputtedString = inputtedString.Trim().ToUpper();
            }

            return inputtedString;
        }

        public static string RemoveHtmlAttribute(this string inputtedString)
        {
            string result = string.Empty;
            if (!string.IsNullOrEmpty(inputtedString) && !string.IsNullOrWhiteSpace(inputtedString))
                result = Regex.Replace(inputtedString, @"\t|\n|\r", "");
            return result;
        }

        public static string StrToUpper(this string inputtedString)
        {
            if (!string.IsNullOrEmpty(inputtedString) && !string.IsNullOrWhiteSpace(inputtedString))
                inputtedString = inputtedString.Trim().ToUpper();
            return inputtedString;
        }

        public static string StrToLower(this string inputtedString)
        {
            if (!string.IsNullOrEmpty(inputtedString) && !string.IsNullOrWhiteSpace(inputtedString))
                inputtedString = inputtedString.Trim().ToLower();
            return inputtedString;
        }

        public static int ConvertToInt32(this string inputtedString)
        {
            if (!string.IsNullOrEmpty(inputtedString) && !string.IsNullOrWhiteSpace(inputtedString))
                return Convert.ToInt32(inputtedString);
            else
                return default(int);
        }

        public static string ConvertToString(this int inputtedValue)
        {
            return Convert.ToString(inputtedValue);
        }
    }
}
