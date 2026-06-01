using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using finsyncapi.Dto;
using finsyncapi.Models;

namespace finsyncapi.BAL.IServices
{
    public interface IAuthService
    {
        Task<ResultDto<SnowFlakeId?>> RegisterAsync(RegisterDto req);
        Task<bool> RegisterResendOtpAsync(ResendOtpDto req);
        Task<TokenResponseDto> RegistrationVerifyOtpAsync(VerifyOtpDto req);
        Task<TokenResponseDto> LoginAsync(LoginDto req);
        Task<bool> LogoutAsync(string Token);
        Task<TokenResponseDto> TokenRefreshAsync(TokenRequestDto req);        
    }
}