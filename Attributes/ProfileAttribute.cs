using System.Net;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using finsyncapi.DAL;
using finsyncapi.Helper;
using finsyncapi.Models;
using static finsyncapi.Helpers.Enums;

namespace finsyncapi.Attributes
{

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class ProfileAttribute : TypeFilterAttribute
    {
        public ProfileRequirement Requirement { get; }

        public ProfileAttribute(ProfileRequirement requirement = ProfileRequirement.Required)
            : base(typeof(ProfileFilter))
        {
            Requirement = requirement;
            Arguments = new object[] { requirement };
        }

        /// <summary>
        /// The actual filter logic. Resolved by DI via TypeFilterAttribute.
        /// </summary>
        private class ProfileFilter : IAsyncActionFilter
        {
            private readonly DapperContext _dapperContext;
            private readonly ProfileRequirement _requirement;

            public ProfileFilter(DapperContext dapperContext, ProfileRequirement requirement)
            {
                _dapperContext = dapperContext;
                _requirement = requirement;
            }

            public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
            {
                // Resolve effective requirement: method-level overrides class-level
                var effectiveRequirement = GetEffectiveRequirement(context) ?? _requirement;

                if (effectiveRequirement == ProfileRequirement.None)
                {
                    await next();
                    return;
                }

                // ProfileRequirement.Required
                var headerValue = context.HttpContext.Request.Headers["x-profileid"].FirstOrDefault();

                if (string.IsNullOrWhiteSpace(headerValue))
                {
                    context.Result = new BadRequestObjectResult(
                        ResponseHelper.Fail<object>("ProfileId is required. Send it in the 'x-profileid' header."));
                    return;
                }

                SnowFlakeId profileId;
                try
                {
                    profileId = SnowFlakeId.Parse(headerValue);
                }
                catch
                {
                    context.Result = new BadRequestObjectResult(
                        ResponseHelper.Fail<object>("Invalid ProfileId format."));
                    return;
                }

                // Get authenticated UserId from claims
                var userIdClaim = context.HttpContext.User.FindFirst(ClaimNames.UserId.GetDescription())?.Value;
                if (string.IsNullOrWhiteSpace(userIdClaim))
                {
                    context.Result = new ObjectResult(
                        ResponseHelper.Fail<object>("Unauthorized."))
                    { StatusCode = (int)HttpStatusCode.Unauthorized };
                    return;
                }

                var userId = SnowFlakeId.Parse(userIdClaim);

                // DB check: profile exists and belongs to this user
                using var connection = _dapperContext.CreateConnection();
                var exists = await connection.ExecuteScalarAsync<bool>(
                    "SELECT EXISTS(SELECT 1 FROM auth.profiles WHERE id = @ProfileId AND user_id = @UserId)",
                    new { ProfileId = profileId.Value, UserId = userId.Value });

                if (!exists)
                {
                    context.Result = new ObjectResult(
                        ResponseHelper.Fail<object>("Invalid or unauthorized ProfileId."))
                    { StatusCode = (int)HttpStatusCode.Forbidden };
                    return;
                }

                // Store validated ProfileId for downstream use
                context.HttpContext.Items["ValidatedProfileId"] = profileId;

                await next();
            }

            /// <summary>
            /// If the action method has its own [Profile] attribute, use that requirement.
            /// Otherwise return null to fall back to the class-level requirement.
            /// </summary>
            private static ProfileRequirement? GetEffectiveRequirement(ActionExecutingContext context)
            {
                if (context.ActionDescriptor is not ControllerActionDescriptor descriptor)
                    return null;

                var methodAttr = descriptor.MethodInfo
                    .GetCustomAttributes(typeof(ProfileAttribute), inherit: true)
                    .OfType<ProfileAttribute>()
                    .FirstOrDefault();

                return methodAttr?.Requirement;
            }
        }
    }
}
