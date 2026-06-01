using finsyncapi.BAL.IServices;
using finsyncapi.BAL.Services;
using finsyncapi.Configuration;
using finsyncapi.Models;

namespace finsyncapi.Extension
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services,IConfiguration configuration)
        {
            services.AddSingleton<IRabbitMqConsumer, RabbitMqConsumer>();
            services.AddSingleton<RabbitMqConsumerBootstrap>();
            services.AddHttpClient<IEmailProvider, ResendEmailProvider>();
            services.Configure<NotificationOptions>(configuration.GetSection("Notification"));
            services.Configure<RabbitMqOptions>(configuration.GetSection("RabbitMQ"));
            services.AddSingleton<IHangfireService, HangfireService>();
            services.AddScoped<NotificationJob>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IRabbitMqPublisher, RabbitMqPublisher>();
            services.AddScoped<ITemplateRenderer, TemplateRenderer>();
            services.AddScoped<IEmailProviderResolver,EmailProviderResolver>();
            services.AddHostedService<RabbitMqConsumerHostedService>();

            return services;
        }
    }
}
