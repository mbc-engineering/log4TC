using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Mbc.Log4Tc.Output.NLog
{
    public static class NLogLog4TcOutputExtensions
    {
        /// <summary>
        /// Register NLog output <see cref="NLogLog4TcOutput"/> as a <see cref="IOutputHandler"/>
        /// </summary>
        public static IServiceCollection AddLog4TcNLogOutput(this IServiceCollection services, string outputName)
        {
            services.TryAddEnumerable(ServiceDescriptor.Singleton<IOutputHandler, NLogLog4TcOutput>(x => new NLogLog4TcOutput(outputName)));
            return services;
        }
    }
}
