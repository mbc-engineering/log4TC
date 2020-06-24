using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Mbc.Log4Tc.Output
{
    public interface IOutputPlugin
    {
        void ConfigureServices(IServiceCollection services, IConfiguration configuration);
    }
}
