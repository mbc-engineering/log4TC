using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Mbc.Log4Tc.Dispatcher
{
    public static class LogDispatcherExtensions
    {
        public static IServiceCollection AddLog4TcDispatcher(this IServiceCollection services, IConfigurationSection outputsConfiguration)
        {
            services.AddHostedService<LogDispatcherService>();

            return services;
        }
    }
}
