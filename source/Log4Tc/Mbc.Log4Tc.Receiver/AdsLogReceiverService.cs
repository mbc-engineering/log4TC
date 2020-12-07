using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Mbc.Log4Tc.Receiver
{
    public class AdsLogReceiverService : IHostedService, IDisposable
    {
        private readonly CancellationTokenSource _stopToken = new CancellationTokenSource();
        private readonly ILogger<AdsLogReceiver> _logger;
        private readonly AdsLogReceiver _adsLogReceiver;
        private Task _connectTask;

        public AdsLogReceiverService(ILogger<AdsLogReceiver> logger, AdsLogReceiver adsLogReceiver)
        {
            _logger = logger;
            _adsLogReceiver = adsLogReceiver;
        }

        public void Dispose()
        {
            _adsLogReceiver.Dispose();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _connectTask = Task.Run(() => ConnectAsync(_stopToken.Token));
            return Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _stopToken.Cancel();
            if (_connectTask != null && !_connectTask.IsCompleted)
            {
                try
                {
                    await _connectTask;
                }
                catch (Exception)
                {
                    // ignored - we just want to ensure no task is running
                }
            }

            _adsLogReceiver.Disconnect();
        }

        private async Task ConnectAsync(CancellationToken ct)
        {
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
                    _logger.LogError(e, "Error connecting log receiver. Waiting for next retry.");
                }

                // Wait for next connect
                await Task.Delay(TimeSpan.FromSeconds(30), ct);
            }
        }
    }
}
