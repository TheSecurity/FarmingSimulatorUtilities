using System;
using System.Globalization;

namespace FarmingSimulatorUtilities.ConsoleApp.Extensions
{
    public static class DateTimeExtensions
    {
        public static bool ToDateTime(this string s, out DateTime? result)
        {
            if (DateTime.TryParseExact(s, "yyyy-MM-dd HH-mm-ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
            {
                result = date;
                return true;
            }

            result = null;
            return false;
        }

        public static string ToDateTimeString(this DateTime d)
            => d.ToString("yyyy-MM-dd HH-mm-ss");
    }
}