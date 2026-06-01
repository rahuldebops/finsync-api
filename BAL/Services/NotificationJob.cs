using finsyncapi.BAL.IServices;
using finsyncapi.Helper;
using finsyncapi.Models;

namespace finsyncapi.BAL.Services
{
    public class NotificationJob
    {
        private readonly IEmailProviderResolver _emailProviderResolver;
        private readonly ITemplateRenderer _templateRenderer;
        public NotificationJob(IEmailProviderResolver emailProviderResolver, ITemplateRenderer templateRenderer)
        {
            _emailProviderResolver = emailProviderResolver;
            _templateRenderer = templateRenderer;

        }
        public async Task SendOtpEmailAsync(string email,string otp)
        {
            var subject = "Your OTP Verification Code";

            var htmlBody = await _templateRenderer.RenderAsync(ProviderConstants.Templates.OtpVerification, new Dictionary<string, string>{{ "Otp", otp }});

            var result = await _emailProviderResolver.SendAsync(new ProviderRequest { Recipient = email, Subject = subject, Content = htmlBody });

            if (!result.IsSuccess)
            {
                throw new Exception(result.FailureReason ?? "Email sending failed");
            }
        }
    }
}
