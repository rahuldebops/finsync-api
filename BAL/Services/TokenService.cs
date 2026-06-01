using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using finsyncapi.BAL.IServices;
using finsyncapi.DAL.Entities;
using System.IdentityModel.Tokens.Jwt;
using finsyncapi.Helper;
using finsyncapi.Models;
using static finsyncapi.Helpers.Enums;

namespace finsyncapi.BAL.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _config;

        public TokenService(IConfiguration config)
        {
            _config = config;
        }

        public string GenerateAccessToken(ClaimModel reqClaims)
        {
            var claims = new[]
            {
                new Claim(ClaimNames.UserId.GetDescription(), reqClaims.UserId.ToString()),
                new Claim(ClaimNames.Email.GetDescription(), reqClaims.Email ?? string.Empty),
                new Claim(ClaimNames.Phone.GetDescription(), reqClaims.PhoneNumber ?? string.Empty),
                new Claim(ClaimNames.TimeZone.GetDescription(), reqClaims.TimeZone),
                new Claim(ClaimNames.Role.GetDescription(), reqClaims.UserRole.ToString()),
            };
        
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"] ?? throw new AppException(Messages.JWTKeyIsMissing)));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddSeconds(AppConstant.ACCESSTOKEN_EXPIRES_SEC),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        }
    }
}