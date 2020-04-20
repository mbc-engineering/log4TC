using Microsoft.Extensions.DependencyInjection;

namespace Mbc.Log4Tc.Dispatcher
{
    public static class LogDispatcherExtensions
    {
        public static IServiceCollection AddLog4TcDispatcher(this IServiceCollection services)
        {
            services.AddHostedService<LogDispatcherService>();
            return services;
        }
    }
}
