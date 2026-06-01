namespace finsyncapi.BAL.IServices
{
    public interface INotificationService
    {
        Task SendOtpEmailAsync(long userid, string email);
        Task SendOtpSmsAsync(long userId, string phoneNumber);
    }
}
