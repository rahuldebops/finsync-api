using finsyncapi.Models;

namespace finsyncapi.BAL.IServices
{
    public interface IEmailProvider
    {
        string ProviderName { get; }

        Task<ProviderResult> SendAsync(ProviderRequest request);
        //Task<ProviderResult> SendAsync(string toEmail,string subject,string htmlBody);
    }
}
