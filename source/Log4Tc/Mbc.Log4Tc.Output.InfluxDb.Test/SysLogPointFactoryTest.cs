using FluentAssertions;
using Mbc.Log4Tc.Model;
using System;
using Xunit;

namespace Mbc.Log4Tc.Output.InfluxDb.Test
{
    public class SysLogPointFactoryTest
    {
        [Fact]
        public void CreatePoint_ShouldLogLogEntry()
        {
            // Arrange
            var factory = new SyslogPointFactory(new InfluxDbOutputSettings());
            var logEntry = new LogEntry
            {
                Level = LogLevel.Info,
                Source = "127.0.0.1.1.1:351",
                TaskName = "Port_851_PlcTask",
                TaskIndex = 1,
                AppName = "Port_851",
                ProjectName = "Plc1",
                Hostname = "PLC1",
                Message = "This is a message.",
                OnlineChangeCount = 2,
                PlcTimestamp = new DateTime(2020, 7, 13, 15, 53, 21, 345),
            };

            // Act
            var point = factory.CreatePoint(logEntry);

            // Assert
            point.ToLineProtocol().Should().Be("syslog,appname=Plc1,facility=Port_851,host=127.0.0.1.1.1:351,hostname=PLC1,level=Info,severity=info,source=127.0.0.1.1.1:351,taskIndex=1,taskName=Port_851_PlcTask facility_code=16i,message=\"This is a message.\",procid=\"2\",severity_code=6i,timestamp=1594648401345000000i,version=1i 1594648401345000");
        }
    }
}
