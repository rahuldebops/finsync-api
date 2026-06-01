using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using finsyncapi.Helper;
using finsyncapi.Models;
using static finsyncapi.Helpers.Enums;
using static finsyncapi.Helpers.ValidationAttributes;

namespace finsyncapi.Dto
{
    public class RegisterDto
    {
        [Required]
        [ValidName]
        public string FullName { get; set; }

        [ValidEmail]
        public string? Email { get; set; }

        public string? PhoneNumber { get; set; }

        [StrongPassword]
        public string? Password { get; set; }

        public string? GoogleId { get; set; }

        [Required]
        public RegistrationProvider Provider { get; set; }

        public string? TimeZone { get; set; }
    }

    public class ResendOtpDto
    {
        [Required]
        public SnowFlakeId UserId { get; set; }
    }

    public class VerifyOtpDto
    {
        [Required]
        public SnowFlakeId UserId { get; set; }

        //[Required]
        //public RegistrationProvider Provider { get; set; }

        //public string? Email { get; set; }

        //public string? PhoneNumber { get; set; }

        [Required]
        public string Otp { get; set; } = string.Empty;

        [Required]
        public string TimeZone { get; set; } = string.Empty;
    }
}