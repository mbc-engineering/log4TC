using Mbc.Log4Tc.Plugin;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Mbc.Log4Tc.Output.Redis
{
    public class RedisLog4TcOutputPlugin : IPlugin
    {
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IOutputFactory, RedisLog4TcOutputFactory>();
        }
    }
}
