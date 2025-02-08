using FakeItEasy;
using Mbc.Log4Tc.Dispatcher;
using Mbc.Log4Tc.Model;
using Mbc.Log4Tc.Receiver;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Mbc.Log4Tc.SmokeTest
{
    internal class Log4TcService : IDisposable
    {
        private readonly AdsLogReceiver _adsLogReceiver;
        private readonly LogDispatcherService _logDispatcher;
        private readonly TestRecordingOutput _output;

        public Log4TcService()
        {
            var adsLogReceiverlogger = new NullLoggerFactory();
            var adsHostnameService = new AdsHostnameService(new NullLogger<AdsHostnameService>());
            _adsLogReceiver = new AdsLogReceiver(adsLogReceiverlogger, adsHostnameService);
            _output = new TestRecordingOutput();
        }

        public List<LogEntry> LoggedEntries => _output.LoggedEntries;

        public void Dispose()
        {
            _adsLogReceiver.Dispose();
        }

        public void Start()
        {
            _logDispatcher.StartAsync(CancellationToken.None).GetAwaiter().GetResult();
            _adsLogReceiver.ConnectServer();
        }

        public void Stop()
        {
            _adsLogReceiver.Disconnect();
        }
    }
}
