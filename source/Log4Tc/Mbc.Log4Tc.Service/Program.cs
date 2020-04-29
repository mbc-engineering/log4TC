using Mbc.Log4Tc.Dispatcher;
using Mbc.Log4Tc.Dispatcher.DispatchExpression;
using Mbc.Log4Tc.Output.NLog;
using Mbc.Log4Tc.Receiver;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
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

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            var hostBuilder = Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration(configure =>
                {
                    // The place to find the appsettings.json
                    configure.SetBasePath(GetAppsettingsBasePath());
                })
                .ConfigureServices((hostContext, services) =>
                {
                    // ToDo: Add from configuration from appsettings
                    services
                        .AddLog4TcDispatchExpression(new DispatchAllLogsToOutput("NLogOutput"))
                        .AddLog4TcAdsLogReceiver()
                        .AddLog4TcNLogOutput("NLogOutput")
                        .AddLog4TcDispatcher();
                });

            if (args.Contains("--service"))
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    hostBuilder.UseWindowsService();
                }
                else
                {
                    throw new PlatformNotSupportedException("Service only in windows system supported.");
                    /* For systemd install first package Microsoft.Extensions.Hosting.Systemd and switch to >= netcoreapp3.0
                     * https://devblogs.microsoft.com/dotnet/net-core-and-systemd/
                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux){
                        hostBuilder.UseSystemd();
                    }
                    */
                }
            }

            return hostBuilder;
        }

        private static string GetAppsettingsBasePath()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return Path.Combine(Environment.ExpandEnvironmentVariables("%programdata%"), "log4TC", "config");
            }
            else
            {
                throw new PlatformNotSupportedException("Service still in windows system supported.");
            }
        }
    }
}
