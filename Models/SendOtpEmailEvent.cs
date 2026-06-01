namespace finsyncapi.Models
{
    public class SendOtpEmailEvent
    {
        public string Email { get; set; } = string.Empty;
        public string Otp { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string Source { get; set; } = string.Empty;
        public string NotificationType { get; set; } = string.Empty;
    }


}
