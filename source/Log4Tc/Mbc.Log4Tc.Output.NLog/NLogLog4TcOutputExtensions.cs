using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Mbc.Log4Tc.Output.NLog
{
    internal static class NLogLog4TcOutputExtensions
    {
        internal static IServiceCollection AddLog4TcNLogOutputSettings(this IServiceCollection services, IConfiguration configuration)
        {
            services
                .Configure<NLogLog4TcOutputConfiguration>(configuration)
                .AddOptions<NLogLog4TcOutputConfiguration>()
                .ValidateDataAnnotations();

            return services;
        }
    }
}
