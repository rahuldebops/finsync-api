namespace finsyncapi.BAL.IServices
{
    public interface IHangfireService
    {
        void EnqueueOtpEmailJob(string email, string otp);
    }
}
