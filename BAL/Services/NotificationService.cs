using finsyncapi.BAL.IServices;
using finsyncapi.DAL.Entities;
using finsyncapi.DAL.IRepositories;
using finsyncapi.Helper;
using finsyncapi.Models;
using static finsyncapi.Helpers.Enums;

namespace finsyncapi.BAL.Services
{
    public class NotificationService : INotificationService
    {
        private readonly NotificationJob _notificationJob;
        private readonly IAuthRepository _authRepo;

        public NotificationService(NotificationJob notificationJob, IAuthRepository authRepo)
        {
            _notificationJob = notificationJob;
            _authRepo = authRepo;
        }

        public async Task SendOtpEmailAsync(long userId, string email)
        {
            var canSend = await _authRepo.CanSendOtpAsync(userId,OtpPurpose.EMAIL_VERIFICATION,email: email);

            if (!canSend)
                throw new AppException(Messages.OtpLimitExceed);

            var (otp, otpHash) = AppHelper.GenerateOtp();

            await _authRepo.CreateEmailOtpAsync(userId,email,otpHash);

            await _notificationJob.SendOtpEmailAsync(email, otp);
        }

        public async Task SendOtpSmsAsync(long userId,string phoneNumber)
        {
            var canSend = await _authRepo.CanSendOtpAsync(userId,OtpPurpose.PHONE_NUMBER_VERIFICATION,phoneNumber: phoneNumber);

            if (!canSend) throw new AppException(Messages.OtpLimitExceed);

            var (otp, otpHash) = AppHelper.GenerateOtp();

            await _authRepo.CreatePhoneOtpAsync(userId,phoneNumber,otpHash);

            /*var payload = new SendOtpSmsEvent
            {
                PhoneNumber = phoneNumber,
                Otp = otp
            };

            await _publisher.PublishAsync(
                payload,
                QueueNames.SendOtpSms
            );*/
        }
    }
}
