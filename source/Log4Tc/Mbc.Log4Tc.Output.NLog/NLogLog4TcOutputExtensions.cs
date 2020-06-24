using Microsoft.Extensions.DependencyInjection;

namespace Mbc.Log4Tc.Output.NLog
{
    internal static class NLogLog4TcOutputExtensions
    {
        /// <summary>
        /// Register NLog output <see cref="NLogLog4TcOutput"/> as a <see cref="IOutputHandler"/>
        /// </summary>
        public static IServiceCollection AddLog4TcNLogOutput(this IServiceCollection services)
        {
            OutputAliases.AddKnownOutputAlias("nlog", typeof(NLogLog4TcOutput));

            return services;
        }

        public static IServiceCollection AddLog4TcNLogOutput(this IServiceCollection services, IConfiguration configuration)
        {
            OutputAliases.AddKnownOutputAlias("nlog", typeof(NLogLog4TcOutput));

            services
                .Configure<NLogLog4TcOutputConfiguration>(configuration)
                .AddOptions<NLogLog4TcOutputConfiguration>()
                .ValidateDataAnnotations();

            return services;
        }
    }
}
