namespace finsyncapi.BAL.Services
{
    using System.Net;
    using System.Security.Claims;
    using Microsoft.AspNetCore.Http;
    using finsyncapi.BAL.IServices;
    using finsyncapi.Helper;
    using finsyncapi.Models;
    using static finsyncapi.Helpers.Enums;

    public class ClaimService : IClaimService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ClaimService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        public UserContext UserContext
        {
            get
            {
                var httpContext = _httpContextAccessor.HttpContext?? throw new AppException (Messages.InvalidAccessToken);

                var user = httpContext.User;

                if (user?.Identity?.IsAuthenticated != true) throw new AppException(Messages.InvalidAccessToken);

                string? Get(string claimType) => user.FindFirst(claimType)?.Value;

                var userIdRaw = Get(ClaimNames.UserId.GetDescription())?? throw new AppException(Messages.InvalidAccessToken);

                var roleRaw = Get(ClaimNames.Role.GetDescription())?? throw new AppException(Messages.InvalidAccessToken);

                var phone = Get(ClaimNames.Phone.GetDescription());
                var email = Get(ClaimNames.Email.GetDescription());
                var timeZone = Get(ClaimNames.TimeZone.GetDescription());

                var profileIdRaw = httpContext.Request.Headers["x-profileid"].FirstOrDefault();
                SnowFlakeId? profileId = null;
                if (!string.IsNullOrWhiteSpace(profileIdRaw))
                {
                    try { profileId = SnowFlakeId.Parse(profileIdRaw); } catch { /* invalid format, leave null */ }
                }

                return new UserContext(SnowFlakeId.Parse(userIdRaw), int.Parse(roleRaw), phone, email, timeZone, profileId);
            }
        }
        /*public ClaimModel CurrentUser
        {
            get
            {
                var user = _httpContextAccessor.HttpContext?.User;

                return new ClaimModel
                {
                    UserId = SnowFlakeId.Parse(user?.FindFirst(ClaimNames.UserId.GetDescription())?.Value ?? throw new AppException(Messages.InvalidAccessToken)),
                    PhoneNumber = user.FindFirst(ClaimNames.Phone.GetDescription())?.Value ?? throw new AppException(Messages.InvalidAccessToken),
                    Email = user.FindFirst(ClaimNames.Email.GetDescription())?.Value ?? throw new AppException(Messages.InvalidAccessToken),
                    UserRole = int.Parse(user.FindFirst(ClaimNames.Role.GetDescription())?.Value ?? throw new AppException(Messages.InvalidAccessToken)),
                    TimeZone = user.FindFirst(ClaimNames.TimeZone.GetDescription())?.Value ?? throw new AppException(Messages.InvalidAccessToken)
                };
            }
        }*/
    }

}
