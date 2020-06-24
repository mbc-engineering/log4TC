using Mbc.Log4Tc.Dispatcher;
using Mbc.Log4Tc.Dispatcher.DispatchExpression;
using Mbc.Log4Tc.Output;
using Mbc.Log4Tc.Output.NLog;
using Mbc.Log4Tc.Receiver;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.Collections.Generic;
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
                    if (!args.Contains("--localconfig"))
                    {
                        // The place to find the appsettings.json
                        configure.SetBasePath(GetAppsettingsBasePath());
                    }
                })
                .ConfigureLogging(loggingBuilder =>
                {
                    loggingBuilder.ClearProviders();

                    var logPath = Path.Combine(GetInternalBasePath(), "service.log");
                    var logger = new LoggerConfiguration()
                        .Enrich.FromLogContext()
                        .WriteTo.Console()
                        .WriteTo.RollingFile(logPath, outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level}] ({SourceContext}) {Message}{NewLine}{Exception}", fileSizeLimitBytes: 1024 * 1024 * 10, retainedFileCountLimit: 5)
                        .CreateLogger();

                    loggingBuilder.AddSerilog(logger: logger, dispose: true);
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services
                        .AddLog4TcDispatchExpression(new DispatchAllLogsToOutput("NLogOutput"))
                        //.AddLog4TcAdsLogReceiver()
                        // ToDo: remove and use confension
                        .AddLog4TcNLogOutput()
                        .AddOutputs(hostContext.Configuration)
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

        private static string GetInternalBasePath()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return Path.Combine(Environment.ExpandEnvironmentVariables("%programdata%"), "log4TC", "internal");
            }
            else
            {
                throw new PlatformNotSupportedException("Service still in windows system supported.");
            }
        }

        private static string GetOutputPluginPath()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
#if DEBUG
                return @"../../outputplugins";
#else
                return "outputplugins"
#endif
            }
            else
            {
                throw new PlatformNotSupportedException("Service still in windows system supported.");
            }
        }
    }
}
