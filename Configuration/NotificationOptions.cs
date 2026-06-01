namespace finsyncapi.Configuration
{
    public class NotificationOptions
    {
        public EmailChannelOptions Email { get; set; } = new();
    }

    public class EmailChannelOptions
    {
        public string Selection { get; set; } = "FAILOVER";
        public List<string> Priority { get; set; } = new();
        public Dictionary<string, ProviderConfig> Providers { get; set; } = new();
    }

    public class ProviderConfig
    {
        public string ApiKey { get; set; } = string.Empty;
        public string FromEmail { get; set; } = string.Empty;
    }
}
