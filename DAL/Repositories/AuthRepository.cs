using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Dapper;
using finsyncapi.BAL.Services;
using finsyncapi.DAL.Entities;
using finsyncapi.DAL.IRepositories;
using finsyncapi.Dto;
using finsyncapi.Helper;
using finsyncapi.Helpers;
using finsyncapi.Models;
using static finsyncapi.Helpers.Enums;

namespace finsyncapi.DAL.Repositories
{
    public class AuthRepository : Repository<Otp, DB1Context>, IAuthRepository
    {
        private readonly DapperContext _dapperContext;
        private readonly ISnowflakeService _snowflake;

        public AuthRepository(DB1Context context, DapperContext dapperContext, ISnowflakeService snowflake) : base(context)
        {
            _dapperContext = dapperContext;
            _snowflake = snowflake; 
        }
        public async Task<ResultDto<SnowFlakeId?>> RegisterUser(User newUser, short provider)
        {
            const string sql = @"SELECT auth.user_create(@payload::jsonb)";

            using var connection = _dapperContext.CreateConnection();

            var payload = JsonSerializer.Serialize(new
            {
                email = newUser.Email,
                phone_number = newUser.PhoneNumber,
                google_id = newUser.GoogleId,
                full_name = newUser.FullName,
                provider = provider,
                user_role = newUser.UserRole,
                password_hash = newUser.PasswordHash,
                auth_user_pk = _snowflake.NextId(),
                auth_profile_pk = _snowflake.NextId(),
                auth_account_pk = _snowflake.NextId(),
                auth_accountprofile_pk = _snowflake.NextId(),
            });

            var jsonResult = await connection.ExecuteScalarAsync<string>(sql, new
            {
                payload
            });
            var res = JsonSerializer.Deserialize<ResultDto<long>>(jsonResult);

            return new ResultDto<SnowFlakeId?>
            {
                Data = res.Data,
                Success = res.Success,
                Message = res.Message
            };
        }
        public async Task<bool> CanSendOtpAsync(long userId,OtpPurpose purpose,string? email = null,string? phoneNumber = null)
        {
            const string sql = @"SELECT auth.can_send_otp(@UserId,@Purpose::int2,@Email,@PhoneNumber,@CooldownSeconds,@MaxResendCount,@WindowHours);";

            using var connection = _dapperContext.CreateConnection();

            return await connection.ExecuteScalarAsync<bool>(sql, new
            {
                UserId = userId,
                Purpose = purpose,
                Email = email,
                PhoneNumber = phoneNumber,
                CooldownSeconds = AppConstant.OTP_RESEND_COOLDOWN_SEC,
                MaxResendCount = AppConstant.OTP_MAX_RESEND_COUNT,
                WindowHours = AppConstant.OTP_MAX_RESEND_WINDOW_HOURS
            });
        }

        public async Task<long> CreateEmailOtpAsync(long userId, string email, string otpHash)
        {
            const string sql = @"WITH invalidated AS (
                            UPDATE auth.otps
                            SET is_used = true
                            WHERE user_id = @UserId
                              AND email = @Email
                              AND purpose = @Purpose
                              AND is_used = false
                            RETURNING id
                        )
                        INSERT INTO auth.otps (user_id,email,purpose,otp_hash,expires_at)
                        VALUES (@UserId,@Email,@Purpose,@OtpHash,@ExpiresAt)
                        RETURNING id;";

            using var connection = _dapperContext.CreateConnection();

            var otpId = await connection.ExecuteScalarAsync<long>(sql, new
            {
                UserId = userId,
                Email = email,
                Purpose = OtpPurpose.EMAIL_VERIFICATION,
                OtpHash = otpHash,
                ExpiresAt = DateTime.UtcNow.AddSeconds(AppConstant.OTP_EXPIRES_SEC)
            });

            return otpId;
        }

        public async Task<long> CreatePhoneOtpAsync(long userId, string phoneNumber, string otpHash)
        {
            const string sql = @"WITH invalidated AS (
                            UPDATE auth.otps
                            SET is_used = true
                            WHERE user_id = @UserId
                              AND phone_number  = @PhoneNumber
                              AND purpose = @Purpose
                              AND is_used = false
                            RETURNING id
                        )
                        INSERT INTO auth.otps (user_id,phone_number,purpose,otp_hash,expires_at)
                        VALUES (@UserId,@PhoneNumber,@Purpose,@OtpHash,@ExpiresAt)
                        RETURNING id;";

            using var connection = _dapperContext.CreateConnection();

            var otpId = await connection.ExecuteScalarAsync<long>(sql, new
            {
                UserId = userId,
                PhoneNumber = phoneNumber,
                Purpose = OtpPurpose.PHONE_NUMBER_VERIFICATION,
                OtpHash = otpHash,
                ExpiresAt = DateTime.UtcNow.AddSeconds(AppConstant.OTP_EXPIRES_SEC)
            });

            return otpId;
        }


        public async Task<ResultDto<bool>> VerifyEmailOtpAsync(long userId, string inputOtp, string otpHash)
        { 
            const string sql = @"WITH otp_row AS (
                                SELECT *
                                FROM auth.otps
                                WHERE user_id = @UserId
                                  AND purpose = @Purpose
                                  AND is_used = false
                                ORDER BY created_at DESC
                                LIMIT 1
                            ),
                            attempt_update AS (
                                UPDATE auth.otps o
                                SET attempts = o.attempts + 1
                                FROM otp_row r
                                WHERE o.id = r.id
                                  AND r.expires_at > now()
                                  AND r.attempts < @MaxAttempts
                                  AND o.otp_hash <> @OtpHash
                                RETURNING o.id
                            ),
                            success_update AS (
                                UPDATE auth.otps o
                                SET is_used = true
                                FROM otp_row r
                                WHERE o.id = r.id
                                  AND r.expires_at > now()
                                  AND o.otp_hash = @OtpHash
                                RETURNING o.id
                            )
                            SELECT
                                CASE
                                    WHEN NOT EXISTS (SELECT 1 FROM otp_row) THEN 'NOT_FOUND'
                                    WHEN (SELECT expires_at FROM otp_row) <= now() THEN 'EXPIRED'
                                    WHEN (SELECT attempts FROM otp_row) >= @MaxAttempts THEN 'MAX_ATTEMPTS'
                                    WHEN EXISTS (SELECT 1 FROM success_update) THEN 'SUCCESS'
                                    ELSE 'INVALID_OTP'
                                END AS status;";

            using var connection = _dapperContext.CreateConnection();

            var status = await connection.ExecuteScalarAsync<string>(sql, new
            {
                UserId = userId,
                Purpose = OtpPurpose.EMAIL_VERIFICATION,
                OtpHash = otpHash,
                MaxAttempts = AppConstant.OTP_MAX_ATTEMPT
            });

            return status switch
            {
                "SUCCESS" => new ResultDto<bool>
                {
                    Success = true,
                    Message = "OTP verified successfully"
                },
                "INVALID_OTP" => throw new AppException(Messages.InvalidOtp),
                "EXPIRED" => throw new AppException(Messages.OtpExpired),
                "MAX_ATTEMPTS" => throw new AppException(Messages.MaximumAttemptsReached),
                _ => throw new AppException(Messages.OtpNotFound)
            };
        }

        private class ProfileRow
        {
            public long ProfileId { get; set; }
            public long UserId { get; set; }
            public string ProfileName { get; set; }
        }        

    }
}