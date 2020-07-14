using FluentAssertions;
using Mbc.Log4Tc.Model;
using NLog;
using NLog.Config;
using NLog.Targets;
using System;
using System.Collections.Generic;
using Xunit;

using NLogLevel = NLog.LogLevel;
using Log4TcLevel = Mbc.Log4Tc.Model.LogLevel;
using NLog.MessageTemplates;

namespace Mbc.Log4Tc.Output.NLog.Test
{
    public class NLogLog4TcOutputTest
    {
        private readonly NLogLogEventTarget _target = new NLogLogEventTarget();
        private readonly NLogLog4TcOutput _output;

        public NLogLog4TcOutputTest()
        {
            _output = new NLogLog4TcOutput(new NLogLog4TcOutputConfiguration());

            var config = new LoggingConfiguration();

            config.AddTarget("target", _target);

            config.AddRule(NLogLevel.Trace, NLogLevel.Fatal, "target");

            LogManager.Configuration = config;

        }

        [Fact]
        public void ProcessLogEntry_SmokeTest()
        {
            // Arrange
            var logEntry = new LogEntry
            {
                Level = Log4TcLevel.Info,
                Source = "source",
                Message = "message with {args} {1}",
                Logger = "logger",
                PlcTimestamp = new DateTime(2020, 7, 8, 14, 23, 45, 163, DateTimeKind.Utc),
                TaskIndex = 42,
                TaskName = "task",
                TaskCycleCounter = 420,
                AppName = "app",
                ProjectName = "project",
                OnlineChangeCount = 24,
            };
            logEntry.Arguments.Add(1, "first arg");
            logEntry.Arguments.Add(2, 4242);
            logEntry.Context.Add("foo", "baz");

            // Act
            _output.ProcesLogEntry(logEntry);

            // Assert
            var logs = _target.Logs;
            logs.Should().HaveCount(1);
            logs[0].Message.Should().Be("message with {args} {1}");
            logs[0].FormattedMessage.Should().Be("message with first arg 4242");
            logs[0].TimeStamp.Should().Be(new DateTime(2020, 7, 8, 14, 23, 45, 163, DateTimeKind.Utc));
            logs[0].LoggerName.Should().Be("logger");
            logs[0].Level.Should().Be(NLogLevel.Info);
            logs[0].Parameters.Should().BeEquivalentTo(new object[] { "first arg", 4242 });
            logs[0].HasProperties.Should().BeTrue();
            logs[0].Properties.Should().BeEquivalentTo(new Dictionary<object, object>
            {
                ["args"] = "first arg",
                ["1"] = 4242,
                ["_TcTaskIdx_"] = 42,
                ["_TcTaskName_"] = "task",
                ["_TcTaskCycleCounter_"] = 420,
                ["_TcAppName_"] = "app",
                ["_TcProjectName_"] = "project",
                ["_TcOnlineChangeCount_"] = 24,
                ["_TcLogSource_"] = "source",
                ["_TcHostname_"] = null,
                ["foo"] = "baz",
            });
            logs[0].MessageTemplateParameters.Should().HaveCount(2).And.BeEquivalentTo(new[] { new MessageTemplateParameter("args", "first arg", null, CaptureType.Normal), new MessageTemplateParameter("1", 4242, null, CaptureType.Normal) });
        }

        [Fact]
        public void NLogLogger_ArgumentHandlingComparison()
        {
            // Arrange
            var logger = LogManager.GetLogger("logger");

            // Act
            logger.Info("message with {args} {1}", "first arg", 4242);

            // Assert
            var logs = _target.Logs;
            logs.Should().HaveCount(1);
            logs[0].Message.Should().Be("message with {args} {1}");
            logs[0].FormattedMessage.Should().Be("message with \"first arg\" 4242");
            logs[0].LoggerName.Should().Be("logger");
            logs[0].Level.Should().Be(NLogLevel.Info);
            logs[0].Parameters.Should().BeEquivalentTo(new object[] { "first arg", 4242 });
            logs[0].HasProperties.Should().BeTrue();
            logs[0].Properties.Should().BeEquivalentTo(new Dictionary<object, object>
            {
                ["args"] = "first arg",
                ["1"] = 4242,
            });
            logs[0].MessageTemplateParameters.Should().HaveCount(2).And.BeEquivalentTo(new[] { new MessageTemplateParameter("args", "first arg", null, CaptureType.Normal), new MessageTemplateParameter("1", 4242, null, CaptureType.Normal) });
        }

        [Fact]
        public void NLogLogger_ArgumentHandlingComparison_WithMixedArguments()
        {
            // Arrange
            var logger = LogManager.GetLogger("logger");

            // Act
            logger.Info("{args} {2} {1} {4} {foo}", 1, 2, 3, 4, 5);

            // Assert
            var logs = _target.Logs;
            logs.Should().HaveCount(1);
            logs[0].FormattedMessage.Should().Be("1 2 3 4 5");
            logs[0].Parameters.Should().BeEquivalentTo(new object[] { 1, 2, 3, 4, 5 });
            logs[0].HasProperties.Should().BeTrue();
            logs[0].Properties.Should().BeEquivalentTo(new Dictionary<object, object>
            {
                ["args"] = 1,
                ["1"] = 3,
                ["2"] = 2,
                ["4"] = 4,
                ["foo"] = 5,
            });
            logs[0].MessageTemplateParameters.Should().HaveCount(5).And.BeEquivalentTo(new[]
            {
                new MessageTemplateParameter("args", 1, null, CaptureType.Normal),
                new MessageTemplateParameter("2", 2, null, CaptureType.Normal),
                new MessageTemplateParameter("1", 3, null, CaptureType.Normal),
                new MessageTemplateParameter("4", 4, null, CaptureType.Normal),
                new MessageTemplateParameter("foo", 5, null, CaptureType.Normal),
            });
        }

        [Fact]
        public void NLogLogger_ArgumentHandlingComparison_WithNumericArguments()
        {
            // Arrange
            var logger = LogManager.GetLogger("logger");

            // Act
            logger.Info("{0} {2} {1} {4} {0}", 1, 2, 3, 4, 5);

            // Assert
            var logs = _target.Logs;
            logs.Should().HaveCount(1);
            logs[0].FormattedMessage.Should().Be("1 3 2 5 1");
            logs[0].Parameters.Should().BeEquivalentTo(new object[] { 1, 2, 3, 4, 5 });
            logs[0].HasProperties.Should().BeFalse();
            logs[0].Properties.Should().BeEmpty();
            logs[0].MessageTemplateParameters.Should().HaveCount(5).And.BeEquivalentTo(new[]
            {
                new MessageTemplateParameter("0", 1, null, CaptureType.Normal),
                new MessageTemplateParameter("2", 3, null, CaptureType.Normal),
                new MessageTemplateParameter("1", 2, null, CaptureType.Normal),
                new MessageTemplateParameter("4", 5, null, CaptureType.Normal),
                new MessageTemplateParameter("0", 1, null, CaptureType.Normal),
            });
        }

        private class NLogLogEventTarget : Target
        {
            public List<LogEventInfo> Logs { get; } = new List<LogEventInfo>();

            protected override void Write(LogEventInfo logEvent)
            {
                Logs.Add(logEvent);
            }
        }
    }
}
