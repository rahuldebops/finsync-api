using System.Net;
using System.Security.Claims;
using finsyncapi.Models;

namespace finsyncapi.Helper
{
    public static class ClaimHelper
    {
        public static UserContext GetClaim(this ClaimsPrincipal user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));

            var userId = long.Parse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new AppException("User ID not found.", HttpStatusCode.Unauthorized));
            var email = user.FindFirst(ClaimTypes.Email)?.Value;
            var phoneNumber = user.FindFirst(ClaimTypes.MobilePhone)?.Value;
            var role = int.TryParse(user.FindFirst(ClaimTypes.Role)?.Value, out var r) ? r : 3;

            return new UserContext(userId, role, phoneNumber, email, null, null);
        }

        
    }
}
