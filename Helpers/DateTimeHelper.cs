using System.Globalization;

namespace finsyncapi.Helpers
{
    public static class DateTimeHelper
    {
        public static string DefaultTimeZone = "UTC";

        public static DateTime ParseUserTimeStringToUtc(string? iso, string? tzId, DateTime? fallback = null)
        {
            if (string.IsNullOrWhiteSpace(iso) || !DateTime.TryParse(iso, null, DateTimeStyles.RoundtripKind, out var dt))
                dt = fallback ?? DateTime.UtcNow;
            var tz = Resolve(tzId);
            return TimeZoneInfo.ConvertTimeToUtc(DateTime.SpecifyKind(dt, DateTimeKind.Unspecified), tz);
        }

        public static string FormatUtcToUserTimeIsoString(DateTime utc, string? tzId) =>
            TimeZoneInfo.ConvertTimeFromUtc(utc, Resolve(tzId)).ToString("yyyy-MM-ddTHH:mm:ss");

        private static TimeZoneInfo Resolve(string? id)
        {
            try { return TimeZoneInfo.FindSystemTimeZoneById(string.IsNullOrWhiteSpace(id) ? DefaultTimeZone : id); }
            catch { return TimeZoneInfo.Utc; }
        }
    }
}
