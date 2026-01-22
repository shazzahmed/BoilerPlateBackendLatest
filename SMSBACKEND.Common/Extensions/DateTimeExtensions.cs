
namespace Common.Extensions
{
    public static class DateTimeExtensions
    {
        // Caching Pakistan Standard Time zone for performance
        private static readonly TimeZoneInfo PakistanTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time");

        public static DateTime ToStartOfDay(this DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 0, 0, 0, DateTimeKind.Utc);
        }

        public static DateTime ToEndOfDay(this DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 23, 59, 59, DateTimeKind.Utc);
        }

        public static DateTime? ToStartOfDay(this DateTime? dateTime)
        {
            return dateTime?.Date.ToStartOfDay();
        }

        public static DateTime? ToEndOfDay(this DateTime? dateTime)
        {
            return dateTime?.Date.ToEndOfDay();
        }

        /// <summary>
        /// Converts time to a specific time zone. Defaults to "Pakistan Standard Time".
        /// </summary>
        public static DateTime ToTimeZoneTime(this DateTime time, string timeZoneId = "Pakistan Standard Time")
        {
            var tzi = timeZoneId == "Pakistan Standard Time" ? PakistanTimeZone : TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
            return TimeZoneInfo.ConvertTimeFromUtc(time, tzi);
        }

        public static DateTime? ToTimeZoneTime(this DateTime? time, string timeZoneId = "Pakistan Standard Time")
        {
            if (time == null)
                return null;
            var tzi = timeZoneId == "Pakistan Standard Time" ? PakistanTimeZone : TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
            return TimeZoneInfo.ConvertTimeFromUtc(time.Value, tzi);
        }

        public static DateTime? ToUTC(this DateTime? time, string timeZoneId)
        {
            if (time == null)
                return null;
            return time.Value.ToUTC(timeZoneId);
        }

        public static DateTime ToUTC(this DateTime time, string timeZoneId)
        {
            time = DateTime.SpecifyKind(time, DateTimeKind.Unspecified);
            return TimeZoneInfo.ConvertTimeToUtc(time, TimeZoneInfo.FindSystemTimeZoneById(timeZoneId));
        }

        /// <summary>
        /// Converts time to a specific TimeZoneInfo.
        /// </summary>
        public static DateTime ToTimeZoneTime(this DateTime time, TimeZoneInfo timeZoneInfo)
        {
            return TimeZoneInfo.ConvertTimeFromUtc(time, timeZoneInfo);
        }

        /// <summary>
        /// Truncates a DateTime to the nearest time span.
        /// </summary>
        public static DateTime Truncate(this DateTime dateTime, TimeSpan timeSpan)
        {
            if (timeSpan == TimeSpan.Zero) return dateTime; // Avoid divide by zero
            if (dateTime == DateTime.MinValue || dateTime == DateTime.MaxValue) return dateTime; // Do not modify guard values
            return dateTime.AddTicks(-(dateTime.Ticks % timeSpan.Ticks));
        }

        /// <summary>
        /// Helper method to convert time to the specified time zone.
        /// </summary>
        private static DateTime ConvertToTimeZone(DateTime time, TimeZoneInfo timeZoneInfo)
        {
            return TimeZoneInfo.ConvertTimeFromUtc(time, timeZoneInfo);
        }
    }
}
