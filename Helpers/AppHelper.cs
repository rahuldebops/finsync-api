using System.ComponentModel;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace finsyncapi.Helper
{
    public static class AppHelper
    {
        // 1. TryParseInt
        public static int? TryParseInt(string? value)
        {
            return int.TryParse(value, out var result) ? result : null;
        }

        // 2. TryParseDateTime
        public static DateTime? TryParseDateTime(string? value, string format = "yyyy-MM-dd")
        {
            return DateTime.TryParseExact(value, format, null, System.Globalization.DateTimeStyles.None, out var result) ? result : null;
        }

        // 3. IsNullOrEmpty
        public static bool IsNullOrEmpty(string? input)
        {
            return string.IsNullOrEmpty(input);
        }

        // 4. SafeTrim
        public static string SafeTrim(string? input)
        {
            return input?.Trim() ?? string.Empty;
        }

        // 5. Generate GUID
        public static string GenerateGuid()
        {
            return Guid.NewGuid().ToString();
        }

        // 6. ConvertToUnixTimestamp
        public static long ToUnixTimestamp(DateTime dateTime)
        {
            return ((DateTimeOffset)dateTime).ToUnixTimeSeconds();
        }

        // 7. FromUnixTimestamp
        public static DateTime FromUnixTimestamp(long timestamp)
        {
            return DateTimeOffset.FromUnixTimeSeconds(timestamp).DateTime;
        }

        // 8. Mask Email (for logging or privacy)
        public static string MaskEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email) || !email.Contains("@"))
                return email;

            var parts = email.Split('@');
            var namePart = parts[0];
            if (namePart.Length <= 2)
                return "***@" + parts[1];

            return namePart.Substring(0, 2) + new string('*', namePart.Length - 2) + "@" + parts[1];
        }

        // 9. ToTitleCase
        public static string ToTitleCase(string input)
        {
            return System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(input.ToLower());
        }

        // 10. IsValidEmail
        public static bool IsValidEmail(string? email)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        // Enum Description
        public static string GetDescription(this Enum value)
        {
            return value.GetType().GetField(value.ToString()).GetCustomAttribute<DescriptionAttribute>()?.Description ?? value.ToString();
        }

        public static bool IsNullOrDefault<T>(T value)
        {
            if (value == null) return true;

            return EqualityComparer<T>.Default.Equals(value, default(T));
        }

        // OTP HELPER
        public static (string otp, string hash) GenerateOtp()
        {
            var otp = AppHelper.GenerateNumericOtp(AppConstant.OTP_LENGTH);
            var hash = AppHelper.HashOtp(otp);

            return (otp, hash);
        }

        public static string GenerateNumericOtp(int length)
        {
            var bytes = new byte[length];
            RandomNumberGenerator.Fill(bytes);

            var otp = new StringBuilder(length);
            foreach (var b in bytes)
            {
                otp.Append((b % 10).ToString());
            }

            return otp.ToString();
        }

        public static string HashOtp(string otp)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(otp);
            var hash = sha256.ComputeHash(bytes);

            return Convert.ToBase64String(hash);
        }

        public static object MergeParameters(object? param1, object param2)
        {
            if (param1 == null) return param2;

            var dict = new Dictionary<string, object>();

            foreach (var prop in param1.GetType().GetProperties())
                dict[prop.Name] = prop.GetValue(param1)!;

            foreach (var prop in param2.GetType().GetProperties())
                dict[prop.Name] = prop.GetValue(param2)!;

            return dict;
        }

        public static string SanitizeQuery(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return query;

            // Remove only trailing semicolons and whitespace
            return query.Trim().TrimEnd(';').TrimEnd();
        }
    }

}
