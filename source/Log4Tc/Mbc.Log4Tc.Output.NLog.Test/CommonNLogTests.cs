using FluentAssertions;
using NLog;
using NLog.Config;
using NLog.Targets;
using System.Collections.Generic;
using Xunit;

namespace Mbc.Log4Tc.Output.NLog.Test
{
    public class CommonNLogTests
    {
        [Fact(Skip = "läuft nicht in Azure Pipeline")]
        public void Test1()
        {
            var config = new LoggingConfiguration();

            var target = new MockTarget();
            config.AddTarget("target", target);

            config.AddRule(LogLevel.Debug, LogLevel.Fatal, "target");

            LogManager.Configuration = config;

            var testLogger = LogManager.GetLogger("Test");

            testLogger.Log(LogLevel.Info, "Arg1={0:N2}", 42.0);
            testLogger.Log(LogLevel.Info, "Arg1={arg1:N2}", 42.0);

            LogManager.Flush();
            target.Logs.Should().HaveCount(2);

            target.Logs[0].Should().Be("Arg1=42.00");
            target.Logs[1].Should().Be("Arg1=42.00");
        }

        private class MockTarget : TargetWithLayout // Same as NullTarget
        {
            public List<string> Logs { get; } = new List<string>();

            protected override void Write(LogEventInfo logEvent)
            {
                Logs.Add(logEvent.FormattedMessage);
            }
        }
    }
}
