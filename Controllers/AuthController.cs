using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using finsyncapi.BAL.IServices;
using finsyncapi.DAL.Entities;
using finsyncapi.Dto;
using finsyncapi.Helper;
using finsyncapi.Models;

namespace finsyncapi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IClaimService _claimService;

        public AuthController(IAuthService authService, IClaimService claimService)
        {
            _authService = authService;
            _claimService = claimService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto req)
        {
            req.TimeZone = ExtractedTimezone();
            ResultDto<SnowFlakeId?> res = await _authService.RegisterAsync(req);
            return Ok(ResponseHelper.Success(res.Data, res.Message));
        }
        [HttpPost("register/resend-otp")]
        public async Task<IActionResult> RegisterResendOtp([FromBody] ResendOtpDto req)
        {
            return Ok(ResponseHelper.Success(await _authService.RegisterResendOtpAsync(req), Messages.OtpSentSuccessfully));
        }

        [HttpPost("register/verify-otp")]
        public async Task<IActionResult> VerifyOtpAsync([FromBody] VerifyOtpDto req)
        {
            return Ok(ResponseHelper.Success(await _authService.RegistrationVerifyOtpAsync(req), Messages.OtpVerifiedSuccessfully));
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto req)
        {
            req.TimeZone = ExtractedTimezone();
            return Ok(ResponseHelper.Success(await _authService.LoginAsync(req), Messages.LoginSuccessfully));
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] TokenRequestDto req)
        {
            return Ok(ResponseHelper.Success(await _authService.LogoutAsync(req.RefreshToken), Messages.LogoutSuccessfully));
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] TokenRequestDto req)
        {
            req.TimeZone = ExtractedTimezone();
            return Ok(ResponseHelper.Success(await _authService.TokenRefreshAsync(req), Messages.LogoutSuccessfully));
        }        

        private string ExtractedTimezone()
        {
            var tz = Request.Headers["X-TimeZone"].ToString();
            try
            {
                return string.IsNullOrWhiteSpace(tz)
                    ? throw new AppException("Missing time zone")
                    : TimeZoneInfo.FindSystemTimeZoneById(tz).Id;
            }
            catch
            {
                throw new AppException("Invalid time zone");
            }
        }
    }

}