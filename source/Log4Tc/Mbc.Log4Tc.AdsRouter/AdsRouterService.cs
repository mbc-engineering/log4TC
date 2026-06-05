using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TwinCAT.Ads.TcpRouter;

namespace Mbc.Log4Tc.AdsRouter;

/// <summary>
/// The Router can be configured in:
/// See: RouterConfig Class https://infosys.beckhoff.com/content/1033/tc3_ads.net/21454899211.html
/// - the appsettings.json with the section "AmsRouter"
/// - Environment variables with the prefix "AmsRouter:"
/// 
/// Should support 
/// - RemoteConnections of RouteConfig
/// - MQTTConnections
/// 
/// See also example: https://github.com/Beckhoff/TF6000_ADS_DOTNET_V5_Samples/blob/main/Sources/RouterSamples/ReadMe.md
/// </summary>
public class AdsRouterService : BackgroundService
{
    private readonly IConfiguration _configuration;
    private readonly ILoggerFactory _loggerFactory;
    private readonly ILogger _logger;

    public AdsRouterService(IConfiguration configuration, ILoggerFactory loggerFactory)
    {
        _configuration = configuration;
        _loggerFactory = loggerFactory;
        _logger = loggerFactory.CreateLogger<AdsRouterService>();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Only if router is configured, try to activate it. Otherwise, do nothing and end the task.
        if (!IsRouterActive())
        {
            return;
        }

        _logger.LogInformation("Starting ADS Router.");
        await StartRouterAsync(stoppingToken);
    }

    private bool IsRouterActive()
    {
        if (_configuration.GetSection("AmsRouter").Exists() && _configuration.GetValue<bool>("AmsRouter:Active"))
        {
            return true;
        }

        return false;
    }

    private async Task StartRouterAsync(CancellationToken ct)
    {
        int delaySeconds = 1;

        while (!ct.IsCancellationRequested)
        {
            try
            {
                AmsTcpIpRouter router = new(_configuration, _loggerFactory);
                using Task routerTask = router.StartAsync(ct);
                _logger.LogInformation("ADS Router started.");
                delaySeconds = 1; // reset delay after successful start

                await routerTask;

                // cancelation in router.StartAsync does not throw exception
                if (ct.IsCancellationRequested)
                {
                    return;
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("ADS Router is stopped.");
                return;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while executing ADS Router. Waiting for next retry in {seconds} second.", delaySeconds);
            }

            // Wait for next connect
            await Task.Delay(TimeSpan.FromSeconds(delaySeconds), ct);

            if (delaySeconds < 64)
            {
                delaySeconds *= 2;
            }
        }
    }
}
