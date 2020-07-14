using FluentAssertions;
using System;
using System.Threading;
using TwinCAT.Ads;
using Xunit;

namespace Mbc.Log4Tc.SmokeTest
{
    public class SmokeTest : IDisposable
    {
        private readonly PlcControl _plcControl;
        private readonly Log4TcService _log4TcService;

        public SmokeTest()
        {
            _plcControl = new PlcControl(AmsAddress.Parse("172.16.23.20.1.1:853"));
            _log4TcService = new Log4TcService();
            _log4TcService.Start();
        }

        public void Dispose()
        {
            _log4TcService.Stop();
            _plcControl.Dispose();
        }

        [Fact(Skip = "not yet finished")]
        public void Test1()
        {
            _plcControl.Stop();
            _plcControl.Reset();
            _plcControl.Start();

            Thread.Sleep(2000);

            var logs = _log4TcService.LoggedEntries;

            logs.Should().HaveCount(1);
            logs[0].Message.Should().Be("F_Log");
        }
    }
}
