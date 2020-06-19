using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Mbc.Log4Tc.Output.NLog
{
    public static class NLogLog4TcOutputExtensions
    {
        /// <summary>
        /// Register NLog output <see cref="NLogLog4TcOutput"/> as a <see cref="IOutputHandler"/>
        /// </summary>
        public static IServiceCollection AddLog4TcNLogOutputType(this IServiceCollection services)
        {
            OutputAliases.AddKnownOutputAlias("nlog", typeof(NLogLog4TcOutputFactory));

            return services;
        }

        public static IServiceCollection AddLog4TcNLogOutputSettings(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddLog4TcNLogOutputType();

            services
                .Configure<NLogLog4TcOutputConfiguration>(configuration)
                .AddOptions<NLogLog4TcOutputConfiguration>()
                .ValidateDataAnnotations();

            return services;
        }
    }
}
