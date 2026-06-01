namespace finsyncapi.Models
{
    public class ProviderRequest
    {
        public string Recipient { get; set; } = string.Empty;

        public string Subject { get; set; } = string.Empty;

        public string Content { get; set; } = string.Empty;

        public Dictionary<string, string> Metadata { get; set; } = new();
    }
}
