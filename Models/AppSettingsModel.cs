namespace finsyncapi.Models
{
    public partial class AppSettingsModel
    {
        public Jwt Jwt { get; set; }
        public ConnectionStrings ConnectionStrings { get; set; }
        public RabbitMq RabbitMq { get; set; }
        public Notification Notification { get; set; }
        public string AllowedHosts { get; set; }
        public MasterOtp MasterOtp { get; set; }
        public AppToggles AppToggles { get; set; }
    }
    public partial class AppToggles
    {
        public bool AllowMasterOTP { get; set; }
        public bool MasterOtpEnabled { get; set; }
    }
    public partial class MasterOtp
    {
        public bool Enabled { get; set; }
        public string Value { get; set; }
    }
    public partial class ConnectionStrings
    {
        public string Db1Connection { get; set; }
    }

    public partial class Jwt
    {
        public string Key { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
    }

    public partial class Notification
    {
        public Email Email { get; set; }
    }

    public partial class Email
    {
        public string Selection { get; set; }
        public List<string> Priority { get; set; }
        public Providers Providers { get; set; }
    }

    public partial class Providers
    {
        public Brevo Resend { get; set; }
        public Brevo Brevo { get; set; }
    }

    public partial class Brevo
    {
        public string ApiKey { get; set; }
        public string FromEmail { get; set; }
    }

    public partial class RabbitMq
    {
        public RabbitMqConnection Connection { get; set; }
    }

    public partial class RabbitMqConnection
    {
        public string Host { get; set; }
        public long Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string VirtualHost { get; set; }
    }
}
