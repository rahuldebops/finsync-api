using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace finsyncapi.Helpers
{
    public static class ValidationAttributes
    {
        public sealed class Iso8601DateAttribute : ValidationAttribute
        {
            private static readonly Regex Regex = new(
                @"^(\d{4})-(\d{2})-(\d{2})(T(\d{2}):(\d{2})(:(\d{2}))?)?(Z|[\+\-]\d{2}:\d{2})?$",
                RegexOptions.Compiled | RegexOptions.CultureInvariant
            );

            public Iso8601DateAttribute() =>
                ErrorMessage = "Invalid ISO 8601 date format.";

            protected override ValidationResult? IsValid(
                object? value,
                ValidationContext context)
            {
                var input = value?.ToString();

                if (string.IsNullOrWhiteSpace(input))
                    return ValidationResult.Success;

                if (!Regex.IsMatch(input))
                    return new ValidationResult(
                        $"{context.DisplayName} must be a valid ISO 8601 date.");

                return DateTimeOffset.TryParse(input, out _)
                    ? ValidationResult.Success
                    : new ValidationResult(
                        $"{context.DisplayName} is not a valid date/time.");
            }
        }

        public sealed class ValidNameAttribute : ValidationAttribute
        {
            private const string RegexPattern =
                @"^[A-Za-z][A-Za-z\s]{1,99}$";

            public ValidNameAttribute() =>
                ErrorMessage = "Full name must contain only letters and spaces.";

            public override bool IsValid(object? value) =>
                !string.IsNullOrWhiteSpace(value?.ToString()) &&
                Regex.IsMatch(value.ToString()!, RegexPattern);
        }

        public sealed class StrongPasswordAttribute : ValidationAttribute
        {
            private const string RegexPattern =
                @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{8,}$";

            public StrongPasswordAttribute() =>
                ErrorMessage = "Password must be at least 8 characters and include uppercase, lowercase, number, and special character.";

            public override bool IsValid(object? value) =>
                !string.IsNullOrWhiteSpace(value?.ToString()) &&
                Regex.IsMatch(value.ToString()!, RegexPattern);
        }

        public sealed class ValidEmailAttribute : ValidationAttribute
        {
            private const string RegexPattern =
                @"^[^@\s]+@[^@\s]+\.[^@\s]+$";

            public ValidEmailAttribute() =>
                ErrorMessage = "Invalid email format.";

            public override bool IsValid(object? value) =>
                !string.IsNullOrWhiteSpace(value?.ToString()) &&
                Regex.IsMatch(value.ToString()!, RegexPattern);
        }
    }
}