using System.Text;
using System.Text.Json;
using finsyncapi.BAL.IServices;
using finsyncapi.Helper;
using finsyncapi.Models;
using Microsoft.Extensions.Options;

namespace finsyncapi.BAL.Services
{
    public class ResendEmailProvider : IEmailProvider
    {
        private readonly AppSettingsModel _settings;
        private readonly HttpClient _httpClient;

        public string ProviderName => ProviderConstants.EmailProviders.Resend;

        public ResendEmailProvider(IOptions<AppSettingsModel> options,HttpClient httpClient)
        {
            _settings = options.Value;
            _httpClient = httpClient;
        }

        public async Task<ProviderResult> SendAsync(ProviderRequest req)
        {
            var provider = _settings.Notification.Email.Providers.Resend;

            var request = new
            {
                from = provider.FromEmail,
                to = new[] { req.Recipient },
                subject = req.Subject,
                html = req.Content
            };

            var httpRequest = new HttpRequestMessage(HttpMethod.Post,"https://api.resend.com/emails");

            httpRequest.Headers.Add("Authorization",$"Bearer {provider.ApiKey}");

            httpRequest.Content = new StringContent(JsonSerializer.Serialize(request),Encoding.UTF8,"application/json");

            var response = await _httpClient.SendAsync(httpRequest);
            var responseBody = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return new ProviderResult
                {
                    IsSuccess = true,
                    ProviderName = ProviderName,
                    ExternalReferenceId = responseBody
                };
            }

            return new ProviderResult
            {
                IsSuccess = false,
                ProviderName = ProviderName,
                FailureReason = responseBody
            };
        }
    }
}