using Mbc.Log4Tc.Dispatcher;
using Mbc.Log4Tc.Output.Graylog;
using Mbc.Log4Tc.Output.InfluxDb;
using Mbc.Log4Tc.Output.NLog;
using Mbc.Log4Tc.Output.Sql;
using Mbc.Log4Tc.Plugin;
using Mbc.Log4Tc.Receiver;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Core;
using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Mbc.Log4Tc.Service
{
    public static class Program
    {
        private static string[] CmdArgs;

        public static async Task Main(string[] args)
        {
            var logPath = Path.Combine(GetInternalBasePath(), "service.log");
            var logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.RollingFile(logPath, outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level}] ({SourceContext}) {Message}{NewLine}{Exception}", fileSizeLimitBytes: 1024 * 1024 * 10, retainedFileCountLimit: 5)
                .CreateLogger();

            try
            {
                logger.Information("Starting log4TC service.");

                await CreateHostBuilder(args, logger)
                    .Build()
                    .RunAsync();
            }
            catch (Exception e)
            {
                logger.Error(e, "Error starting log4TC service.");
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args, Logger logger)
        {
            CmdArgs = args;
            var hostBuilder = Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration(configure =>
                {
                    if (!IsLocalConfig())
                    {
                        // The place to find the appsettings.json
                        configure.SetBasePath(GetAppsettingsBasePath());
                    }
                })
                .ConfigureLogging(loggingBuilder =>
                {
                    loggingBuilder.ClearProviders();
                    loggingBuilder.AddSerilog(logger: logger, dispose: true);
                })
                .ConfigureServices((hostContext, services) =>
                {
                    var plugins = new IPlugin[]
                    {
                        new NLogLog4TcOutputPlugin(),
                        new GraylogLog4TcOutputPlugin(),
                        new InfluxDbLog4TcOutputPlugin(),
                        new SqlLog4TcOutputPlugin(),
                    };

                    foreach (var plugin in plugins)
                    {
                        plugin.ConfigureServices(services, hostContext.Configuration);
                    }

                    // TODO plugin funktioniert momentan nicht mit dritt-Nugets
                    //services
                    //    .AddPlugins(GetPluginPath())
                    //    // ToDo: Differenziate output / input / ... configuration in PluginBuilder
                    //    .AddOutputs(hostContext.Configuration);

                    services
                        .AddLog4TcAdsLogReceiver()
                        .AddLog4TcDispatcher();
                });

            if (args.Contains("--service", StringComparer.InvariantCultureIgnoreCase))
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

        private static string GetPluginPath()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                if (IsLocalConfig())
                {
                    return @".";
                }
                else
                {
                    return "plugins";
                }
            }
            else
            {
                throw new PlatformNotSupportedException("Service still in windows system supported.");
            }
        }

        private static bool IsLocalConfig()
        {
            return CmdArgs.Contains("--localconfig", StringComparer.InvariantCultureIgnoreCase);
        }
    }
}
