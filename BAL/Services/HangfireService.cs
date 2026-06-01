using finsyncapi.BAL.IServices;
using finsyncapi.Models;
using Hangfire;
using Microsoft.Extensions.Options;

namespace finsyncapi.BAL.Services
{
    public class HangfireService : IHangfireService
    {
        private readonly AppSettingsModel _appSettings;
        public HangfireService(IOptions<AppSettingsModel> options)
        {
            _appSettings = options.Value;
        }

        public void EnqueueOtpEmailJob(string email, string otp)
        {
            BackgroundJob.Enqueue<NotificationJob>(x => x.SendOtpEmailAsync(email, otp));
        }
    }
}
