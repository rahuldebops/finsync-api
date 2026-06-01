namespace finsyncapi.Models
{
    public class ProviderResult
    {
        public bool IsSuccess { get; set; }

        public string ProviderName { get; set; } = string.Empty;

        public string? FailureReason { get; set; }

        public string? ExternalReferenceId { get; set; }
    }
}
