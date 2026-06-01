using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;
using finsyncapi.BAL.IServices;
using finsyncapi.DAL.Entities;
using finsyncapi.DAL.IRepositories;
using finsyncapi.Dto;
using finsyncapi.Helper;
using finsyncapi.Models;
using static finsyncapi.Helpers.Enums;
using Humanizer;
using System.Xml.Linq;
using finsyncapi.DAL;
using finsyncapi.DAL.Repositories;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using static System.Net.WebRequestMethods;
using Microsoft.Extensions.Options;

namespace finsyncapi.BAL.Services
{
    public class AuthService : IAuthService
    {
        private readonly INotificationService _notificationService;
        private readonly IMasterRepository _masterRepo;
        private readonly IUserRepository _userRepo;
        private readonly ITokenRepository _tokenRepo;
        private readonly ITokenService _tokenService;
        private readonly IAuthRepository _authRepo;
        private readonly IUnitOfWork<DB1Context> _unitOfWorkDB1;
        private readonly ISnowflakeService _snowflakeService;
        private readonly AppSettingsModel _appSettings;

        public AuthService(IUnitOfWork<DB1Context> unitOfWorkDB1, IUserRepository userRepo, ITokenRepository tokenRepo, ITokenService tokenService, IMasterRepository masterRepo, IAuthRepository authRepo, ISnowflakeService snowflakeService, INotificationService notificationService, IOptions<AppSettingsModel> options)
        {
            _masterRepo = masterRepo;
            _userRepo = userRepo;
            _tokenRepo = tokenRepo;
            _tokenService = tokenService;
            _authRepo = authRepo;
            _notificationService = notificationService;
            _unitOfWorkDB1 = unitOfWorkDB1;
            _snowflakeService = snowflakeService;
            _appSettings = options.Value;
        }
        public async Task<ResultDto<SnowFlakeId?>> RegisterAsync(RegisterDto req)
        {
            ValidateRegistration(req);

            User? existingUser = null;

            if (!string.IsNullOrWhiteSpace(req.Email))
            {
                existingUser = await _userRepo.GetSingleOrDefaultWithSelectedColoumnAysnc(x => new User { Id = x.Id, Email = x.Email, IsVerified = x.IsVerified }, x => x.Email == req.Email);
            }
            else if (!string.IsNullOrWhiteSpace(req.PhoneNumber))
            {
                existingUser = await _userRepo.GetSingleOrDefaultWithSelectedColoumnAysnc(x => new User { Id = x.Id, PhoneNumber = x.PhoneNumber, IsVerified = x.IsVerified }, x => x.PhoneNumber == req.PhoneNumber);
            }

            if (existingUser != null)
            {
                if (!existingUser.IsVerified)
                {
                    switch (req.Provider)
                    {
                        case RegistrationProvider.Email:
                            await _notificationService.SendOtpEmailAsync(existingUser.Id,existingUser.Email!);
                            break;

                        case RegistrationProvider.Phone:
                            await _notificationService.SendOtpSmsAsync(existingUser.Id,existingUser.PhoneNumber!);
                            break;
                    }
                    return new ResultDto<SnowFlakeId?> { Data = existingUser.Id, Message = Messages.OtpSentSuccessfully };
                }
                return new ResultDto<SnowFlakeId?> { Data = existingUser.Id, Message = Messages.UserAlreadyVerified};
            }

            var user = new User
            {
                FullName = req.FullName,
                Email = req.Email,
                PhoneNumber = req.PhoneNumber,
                GoogleId = req.GoogleId,
                UserRole = (short)UserRoleEnum.ENDUSER,
                PasswordHash = req.Provider == RegistrationProvider.Google ? null : BCrypt.Net.BCrypt.HashPassword(req.Password),

                IsVerified = req.Provider == RegistrationProvider.Google
            };

            var createdUser = await _authRepo.RegisterUser(user,(short)req.Provider);

            if (!createdUser.Success)
                throw new AppException(createdUser.Message);

            switch (req.Provider)
            {
                case RegistrationProvider.Email:
                    await _notificationService.SendOtpEmailAsync(createdUser.Data!.Value,req.Email!);
                    return new ResultDto<SnowFlakeId?> { Data = createdUser.Data, Message = Messages.OtpSentSuccessfully };
                /*case RegistrationProvider.Phone:
                    await _notificationService.SendOtpSmsAsync(
                        createdUser.Data,
                        req.PhoneNumber!
                    );
                    return Messages.OtpSentSuccessfully;

                case RegistrationProvider.Google:
                    return "Google registration completed successfully";*/

                default: throw new AppException("Invalid registration provider");
            }
        }

        public async Task<bool> RegisterResendOtpAsync(ResendOtpDto req)
        {
            var user = await _userRepo.GetSingleWithSelectedColoumnAysnc(x => new { Id = x.Id, Email = x.Email, PhoneNumber = x.PhoneNumber, IsVerified = x.IsVerified }, x => x.Id == req.UserId);

            if (user.IsVerified) throw new AppException(Messages.UserAlreadyVerified);


            if (!string.IsNullOrWhiteSpace(user.Email))
            {
                await _notificationService.SendOtpEmailAsync(user.Id, user.Email);
            }
            else if (!string.IsNullOrWhiteSpace(user.PhoneNumber))
            {
                await _notificationService.SendOtpSmsAsync(user.Id, user.PhoneNumber);
            }
            else
            {
                throw new AppException(Messages.SomethingWentWrong);
            }
            return true;
        }

        public async Task<TokenResponseDto> RegistrationVerifyOtpAsync(VerifyOtpDto req)
        {
            var user = await _userRepo.GetSingleWithSelectedColoumnAysnc(x => new User { Id = x.Id, PhoneNumber = x.PhoneNumber, Email = x.Email, UserRole = x.UserRole, IsVerified = x.IsVerified}, x => x.Id == req.UserId);
            if (user.IsVerified) throw new AppException(Messages.UserAlreadyVerified);

            bool isMasterOtp = false;
            var appToggles = _appSettings.AppToggles;
            if (appToggles != null && (appToggles.AllowMasterOTP || appToggles.MasterOtpEnabled))
            {
                var now = DateTime.UtcNow;
                string expectedOtp = $"{now:ddMM}{now.Hour / 10}{(now.Hour % 10) % 3}";
                isMasterOtp = req.Otp == expectedOtp;
            }

            if (!isMasterOtp)
                await _authRepo.VerifyEmailOtpAsync(req.UserId, req.Otp, AppHelper.HashOtp(req.Otp));

            user.IsVerified = true;

            _userRepo.UpdatePartial(user,x => x.IsVerified);

            await _userRepo.SaveChangesAsync();

            var claim = new ClaimModel(
                userId: user.Id,
                userRole: user.UserRole,
                email: user.Email,
                phoneNumber: user.PhoneNumber,
                timeZone: req.TimeZone
            );

            return await GenerateTokensAsync(claim);
        }

        public async Task<TokenResponseDto> LoginAsync(LoginDto req)
        {
            User? user = await _userRepo.GetSingleOrDefaultWithSelectedColoumnAysnc(
                x => new User {
                    Id = x.Id,
                    PasswordHash = x.PasswordHash,
                    Email = x.Email,
                    UserRole = x.UserRole
                },
                x => x.Email == req.Email
            );
        
            if (user == null || !BCrypt.Net.BCrypt.Verify(req.Password, user.PasswordHash))
                throw new AppException(Messages.InvalidCredentials, HttpStatusCode.Unauthorized);

            var claim = new ClaimModel(
                userId: user.Id,
                userRole: user.UserRole,
                email: user.Email,
                phoneNumber: user.PhoneNumber,
                timeZone: req.TimeZone
            );

            return await GenerateTokensAsync(claim);
        }

        public async Task<bool> LogoutAsync(string Token)
        {
            RefreshToken? token = await _tokenRepo.GetSingleOrDefaultWithSelectedColoumnAysnc(
                x => new RefreshToken{
                    IsUsed = x.IsUsed,
                    IsRevoked = x.IsRevoked
                },
                x => x.Token == Token
            );
            if (token != null)
                await _tokenRepo.InvalidateAsync(token);
            return true;
        }

        public async Task<TokenResponseDto> TokenRefreshAsync(TokenRequestDto req)
        {
            RefreshToken? refreshToken = await _tokenRepo.GetSingleOrDefaultWithSelectedColoumnAysnc(
                x => new RefreshToken {
                    IsUsed = x.IsUsed,
                    IsRevoked = x.IsRevoked,
                    ExpiresAt = x.ExpiresAt,
                    UserId = x.UserId
                },
                x => x.Token == req.RefreshToken
            );
            if (refreshToken == null || refreshToken.IsUsed || refreshToken.IsRevoked || refreshToken.ExpiresAt < DateTime.UtcNow)
                throw new AppException(Messages.InvalidRefreshToken);

            await _tokenRepo.InvalidateAsync(refreshToken);

            User user = await _userRepo.GetSingleWithSelectedColoumnAysnc(
                x => new User {
                    Id = x.Id,
                    Email = x.Email,
                    PhoneNumber = x.PhoneNumber,
                    UserRole = x.UserRole
                },
                x => x.Id == refreshToken.UserId
            );
            var claim = new ClaimModel(
                userId: user.Id,
                userRole: user.UserRole,
                email: user.Email,
                phoneNumber: user.PhoneNumber,
                timeZone: req.TimeZone
            );

            return await GenerateTokensAsync(claim);
        }

        private async Task<TokenResponseDto> GenerateTokensAsync(ClaimModel newClaim)
        {
            var accessToken = _tokenService.GenerateAccessToken(newClaim);
            var refreshToken = _tokenService.GenerateRefreshToken();

            var tokenModel = new RefreshToken
            {
                Token = refreshToken,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddSeconds(AppConstant.REFRESHTOKEN_EXPIRES_SEC),
                IsUsed = false,
                IsRevoked = false,
                UserId = newClaim.UserId
            };

            await _tokenRepo.AddAsync(tokenModel);
            await _tokenRepo.SaveChangesAsync();

            return new TokenResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }
       
        
        // HELPER
        private void ValidateRegistration(RegisterDto req)
        {
            switch (req.Provider)
            {
                case RegistrationProvider.Email:
                    if (string.IsNullOrWhiteSpace(req.Email))
                        throw new AppException("Email is required");

                    if (string.IsNullOrWhiteSpace(req.Password))
                        throw new AppException("Password is required");

                    break;

                case RegistrationProvider.Phone:
                    if (string.IsNullOrWhiteSpace(req.PhoneNumber))
                        throw new AppException("Phone number is required");

                    if (string.IsNullOrWhiteSpace(req.Password))
                        throw new AppException("Password is required");

                    break;

                case RegistrationProvider.Google:
                    if (string.IsNullOrWhiteSpace(req.GoogleId))
                        throw new AppException("GoogleId is required");

                    break;

                default:
                    throw new AppException("Invalid registration provider");
            }
        }

    }
}