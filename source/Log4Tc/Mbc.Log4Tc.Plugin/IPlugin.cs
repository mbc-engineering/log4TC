using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Mbc.Log4Tc.Plugin
{
    public interface IPlugin
    {
        void ConfigureServices(IServiceCollection services, IConfiguration configuration);
    }
}
