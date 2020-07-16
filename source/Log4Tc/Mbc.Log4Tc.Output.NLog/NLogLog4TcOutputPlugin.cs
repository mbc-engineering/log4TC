using Mbc.Log4Tc.Plugin;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Mbc.Log4Tc.Output.NLog
{
    public class NLogLog4TcOutputPlugin : IPlugin
    {
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            NLogLog4TcOutputInitializer.Setup();

            services.AddSingleton<IOutputFactory, NLogLog4TcOutputFactory>();
        }
    }
}
