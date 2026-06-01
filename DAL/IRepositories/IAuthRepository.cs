using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using finsyncapi.DAL.Entities;
using finsyncapi.Dto;
using finsyncapi.Models;
using static finsyncapi.Helpers.Enums;

namespace finsyncapi.DAL.IRepositories
{
    public interface IAuthRepository : IRepository<Otp>
    {
        Task<ResultDto<SnowFlakeId?>> RegisterUser(User newUser, short provider);
        Task<bool> CanSendOtpAsync(long userId, OtpPurpose purpose, string? email = null, string? phoneNumber = null);
        Task<long> CreateEmailOtpAsync(long userId, string email, string otpHash);
        Task<long> CreatePhoneOtpAsync(long userId, string phoneNumber, string otpHash);

        Task<ResultDto<bool>> VerifyEmailOtpAsync(long userId, string inputOtp, string otpHash);        
    }
}