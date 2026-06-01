using finsyncapi.Models;

namespace finsyncapi.BAL.IServices
{
    public interface IEmailProviderResolver
    {
        Task<ProviderResult> SendAsync(ProviderRequest request);
    }
}