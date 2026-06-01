using finsyncapi.BAL.IServices;
using finsyncapi.Helper;
using finsyncapi.Models;
using Microsoft.Extensions.Options;
using static finsyncapi.Helpers.Enums;

namespace finsyncapi.BAL.Services
{
    public class EmailProviderResolver : IEmailProviderResolver
    {
        private readonly IEnumerable<IEmailProvider> _providers;
        private readonly AppSettingsModel _settings;

        public EmailProviderResolver(IEnumerable<IEmailProvider> providers,IOptions<AppSettingsModel> options)
        {
            _providers = providers;
            _settings = options.Value;
        }

        public async Task<ProviderResult> SendAsync(ProviderRequest request)
        {
            var emailConfig = _settings.Notification.Email;
            var selection = emailConfig.Selection;
            var priority = emailConfig.Priority;

            if (selection == ProviderConstants.Selection.Primary)
            {
                var primaryProvider = _providers.FirstOrDefault(x => x.ProviderName == priority.First());

                if (primaryProvider is null)
                    throw new Exception(Messages.PrimaryEmailProviderNotFound);

                return await primaryProvider.SendAsync(request);
            }

            foreach (var providerName in priority)
            {
                var provider = _providers.FirstOrDefault(x => x.ProviderName == providerName);

                if (provider is null)
                    continue;

                var result = await provider.SendAsync(request);

                if (result.IsSuccess)
                    return result;
            }

            throw new AppException(Messages.AllEmailProvidersFailed);
        }
    }
}