using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Mbc.Log4Tc.Receiver
{
    public class AdsLogReceiverService : IHostedService, IDisposable
    {
        private readonly AdsLogReceiver _adsLogReceiver;

        public AdsLogReceiverService(AdsLogReceiver adsLogReceiver)
        {
            _adsLogReceiver = adsLogReceiver;
        }

        public void Dispose()
        {
            _adsLogReceiver.Dispose();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _adsLogReceiver.ConnectServer();
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _adsLogReceiver.Disconnect();
            return Task.CompletedTask;
        }
    }
}
