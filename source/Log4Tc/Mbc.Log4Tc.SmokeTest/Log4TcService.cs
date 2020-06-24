using Mbc.Common;
using Mbc.Log4Tc.Dispatcher;
using Mbc.Log4Tc.Model;
using Mbc.Log4Tc.Receiver;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading;

namespace Mbc.Log4Tc.SmokeTest
{
    public class Log4TcService
    {
        private readonly AdsLogReceiver _adsLogReceiver;
        private readonly LogDispatcherService _logDispatcher;
        private readonly TestRecordingOutput _output;
        private readonly ILogger<LogDispatcherService> _logger;

        public Log4TcService()
        {
            _logger = A.Fake<ILogger<LogDispatcherService>>();
            _adsLogReceiver = new AdsLogReceiver();
            _output = new TestRecordingOutput();
            _logDispatcher = new LogDispatcherService(Enumerables.Yield(_adsLogReceiver), Enumerables.Yield(_output), Enumerables.Yield(new DispatchAllLogsToOutput("TestOutput")));
        }

        public List<LogEntry> LoggedEntries => _output.LoggedEntries;

        public void Start()
        {
            _logDispatcher.StartAsync(CancellationToken.None).GetAwaiter().GetResult();
            _adsLogReceiver.Connect();
        }

        public void Stop()
        {
            _adsLogReceiver.Disconnect();
        }
    }
}
