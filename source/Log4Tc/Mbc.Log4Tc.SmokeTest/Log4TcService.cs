using Mbc.Common;
using Mbc.Log4Tc.Dispatcher;
using Mbc.Log4Tc.Dispatcher.DispatchExpression;
using Mbc.Log4Tc.Model;
using Mbc.Log4Tc.Receiver;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System.Collections.Generic;
using System.Threading;

namespace Mbc.Log4Tc.SmokeTest
{
    class Log4TcService
    {
        private readonly AdsLogReceiver _adsLogReceiver;
        private readonly LogDispatcherService _logDispatcher;
        private readonly TestRecordingOutput _output;

        public Log4TcService()
        {

            _adsLogReceiver = new AdsLogReceiver(new NullLogger<AdsLogReceiver>());
            _output = new TestRecordingOutput();
            _logDispatcher = new LogDispatcherService(new NullLogger<LogDispatcherService>(), Enumerables.Yield(_adsLogReceiver), Enumerables.Yield(_output), Enumerables.Yield(new DispatchAllLogsToOutput("TestOutput")));
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
