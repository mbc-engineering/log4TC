using Mbc.Log4Tc.Dispatcher;
using Mbc.Log4Tc.Dispatcher.DispatchExpression;
using Mbc.Log4Tc.Output.NLog;
using Mbc.Log4Tc.Receiver;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;

namespace Mbc.Log4Tc.Service
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            await CreateHostBuilder(args)
                .Build()
                .RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    // ToDo: Add from configuration from appsettings
                    services
                        .AddLog4TcDispatchExpression(new DispatchAllLogsToOutput("NLogOutput"))
                        .AddLog4TcAdsLogReceiver()
                        .AddLog4TcNLogOutput("NLogOutput")
                        .AddLog4TcDispatcher();
                });
    }
}
