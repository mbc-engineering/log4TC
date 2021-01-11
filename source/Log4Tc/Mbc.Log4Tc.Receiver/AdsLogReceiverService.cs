using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Mbc.Log4Tc.Receiver
{
    public class AdsLogReceiverService : BackgroundService, IDisposable
    {
        private readonly ILogger _logger;
        private readonly AdsLogReceiver _adsLogReceiver;

        public AdsLogReceiverService(ILogger<AdsLogReceiver> logger, AdsLogReceiver adsLogReceiver)
        {
            _logger = logger;
            _adsLogReceiver = adsLogReceiver;
        }

        public override void Dispose()
        {
            base.Dispose();
            _adsLogReceiver.Dispose();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Starting ADS log receiver.");
            await ConnectAsync(stoppingToken);
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _adsLogReceiver.Disconnect();
            await base.StopAsync(cancellationToken);
        }

        private async Task ConnectAsync(CancellationToken ct)
        {
            int delaySeconds = 1;

            while (!ct.IsCancellationRequested)
            {
                try
                {
                    _adsLogReceiver.Connect();
                    _logger.LogInformation("Log receiver connected.");
                    return;
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Error connecting log receiver. Waiting for next retry in {seconds} second.", delaySeconds);
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
}
