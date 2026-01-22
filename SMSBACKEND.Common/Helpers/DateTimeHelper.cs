using System.Collections.ObjectModel;

namespace Common.Helpers
{
    public static class DateTimeHelper
    {
        public static TimeZoneInfo GetTimesZone(string Id)
        {
            return GetTimesZones().Where(c => c.Id.Equals(Id)).FirstOrDefault();
        }

        public static ReadOnlyCollection<TimeZoneInfo> GetTimesZones()
        {
            return TimeZoneInfo.GetSystemTimeZones();
        }

        public static DateTime GetFirstDayOfMonth(DateTime date)
        {
            return new DateTime(date.Year, date.Month, 1);
        }

        public static DateTime GetLastDayOfMonth(DateTime date)
        {
            return new DateTime(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month));
        }
    }
}
