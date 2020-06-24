using Microsoft.Extensions.DependencyInjection;

namespace Mbc.Log4Tc.Output.NLog
{
    public static class NLogLog4TcOutputExtensions
    {
        /// <summary>
        /// Register NLog output <see cref="NLogLog4TcOutput"/> as a <see cref="IOutputHandler"/>
        /// </summary>
        public static IServiceCollection AddLog4TcNLogOutput(this IServiceCollection services)
        {
            services.AddSingleton<IOutputFactory, NLogLog4TcOutputFactory>();

            return services;
        }
    }
}
