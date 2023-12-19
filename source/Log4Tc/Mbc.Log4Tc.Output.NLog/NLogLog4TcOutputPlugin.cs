using Mbc.Log4Tc.Plugin;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Mbc.Log4Tc.Output.NLog
{
    public class NLogLog4TcOutputPlugin : IPlugin
    {
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IOutputFactory, NLogLog4TcOutputFactory>();

            // Example see: https://blog.skrots.com/nlog-implementation-with-azure-application-insights/?feed_id=1833&_unique_id=64f62b11bebcc
            // See Application Insights logging adapters: https://github.com/microsoft/ApplicationInsights-dotnet/blob/c420e04562876791f27a61644760d7b7512832d9/LOGGING/README.md#nlog
            services.AddApplicationInsightsTelemetryWorkerService();
        }
    }
}
